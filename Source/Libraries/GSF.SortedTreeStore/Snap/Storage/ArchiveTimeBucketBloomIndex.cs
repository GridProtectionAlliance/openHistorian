//******************************************************************************************************
//  ArchiveTimeBucketBloomIndex.cs - Gbtc
//
//  Copyright © 2026, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/21/2026 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using GSF.Diagnostics;

namespace GSF.Snap.Storage;

// Versioned independently of .pidx. Omitted buckets are empty only in a complete,
// identity-checked index. A disabled or capped builder never publishes a partial index.
internal sealed class ArchiveTimeBucketBloomIndex
{
    internal const string Extension = ".bidx";
    internal const int MaximumBytes = 64 * 1024 * 1024;
    internal const int MaximumBuckets = 100000;
    private const int FileFormatSignature = 0x314D4C42; // ASCII "BLM1" (bloom, version 1), little endian
    private readonly ulong m_width;
    private readonly Bucket[] m_buckets;

    internal sealed class Bucket
    {
        internal ulong Ordinal;
        internal int Probes;
        internal byte[] Bits;
    }

    private ArchiveTimeBucketBloomIndex(ulong width, Bucket[] buckets)
    {
        m_width = width; 
        m_buckets = buckets;
    }

    internal bool MayContain(ulong id, ulong lower, ulong upper)
    {
        if (lower > upper)
            return false;
        
        ulong first = lower / m_width, last = upper / m_width;
        int lo = 0, hi = m_buckets.Length;
        
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            
            if (m_buckets[mid].Ordinal < first)
                lo = mid + 1;
            else
                hi = mid;
        }
        
        for (int i = lo; i < m_buckets.Length && m_buckets[i].Ordinal <= last; i++)
        {
            if (Contains(m_buckets[i], id))
                return true;
        }

        return false;
    }

    // SplitMix64 finalizer, two independent seeds, double hashing. All arithmetic is
    // explicitly unchecked so serialized indexes behave identically on both runtimes.
    private static ulong Mix(ulong value)
    {
        unchecked
        {
            value = (value ^ (value >> 30)) * 0xbf58476d1ce4e5b9UL;
            value = (value ^ (value >> 27)) * 0x94d049bb133111ebUL;
            return value ^ (value >> 31);
        }
    }

    private static bool Contains(Bucket bucket, ulong id)
    {
        ulong a = Mix(id ^ 0x9e3779b97f4a7c15UL), b = Mix(id ^ 0xd1b54a32d192ed03UL) | 1;
        ulong mask = (ulong)bucket.Bits.Length * 8 - 1;
        
        for (int k = 0; k < bucket.Probes; k++)
        {
            ulong bit = unchecked(a + (ulong)k * b) & mask;
            
            if ((bucket.Bits[(int)(bit >> 3)] & (1 << (int)(bit & 7))) == 0)
                return false;
        }

        return true;
    }

    internal sealed class Builder
    {
        private readonly TimeBucketBloomFilterSettings m_settings;
        private readonly List<Bucket> m_buckets = [];
        private readonly HashSet<ulong> m_ids = [];
        private ulong m_ordinal;
        private int m_bytes = 80;
        private bool m_disabled;

        internal Builder(TimeBucketBloomFilterSettings settings)
        {
            m_settings = settings;
        }
        
        internal void Add(ulong timestamp, ulong id)
        {
            if (m_disabled)
                return;
            
            ulong ordinal = timestamp / ((ulong)m_settings.BucketDurationSeconds * (ulong)TimeSpan.TicksPerSecond);
            
            if (m_ids.Count != 0 && ordinal != m_ordinal)
            {
                if (ordinal < m_ordinal)
                {
                    Disable(); 
                    return;
                }
                
                Flush();
                
                if (m_disabled) 
                    return;
            }

            m_ordinal = ordinal;
            m_ids.Add(id);
            
            if (m_ids.Count > ArchivePointIDIndex.MaximumIDs) 
                Disable();
        }

        private void Disable()
        {
            m_disabled = true; 
            m_ids.Clear(); 
            m_buckets.Clear();
        }
        private void Flush()
        {
            if (m_ids.Count == 0 || m_disabled) 
                return;
            
            double desired = -m_ids.Count * Math.Log(m_settings.FalsePositiveProbability) / (Math.Log(2) * Math.Log(2));
            int bytes = 8;
            
            while (bytes * 8.0D < desired && bytes < MaximumBytes) 
                bytes *= 2;

            if (bytes > m_settings.MaximumIndexSizeMiB * 1024 * 1024 - m_bytes - 16 || m_buckets.Count == MaximumBuckets)
            {
                Disable(); 
                return;
            }

            Bucket bucket = new()
            {
                Ordinal = m_ordinal, 
                Bits = new byte[bytes], 
                Probes = Math.Max(1, Math.Min(16, (int)Math.Round(bytes * 8.0 / m_ids.Count * Math.Log(2))))
            };

            ulong mask = (ulong)bytes * 8 - 1;
            
            foreach (ulong id in m_ids)
            {
                ulong a = Mix(id ^ 0x9e3779b97f4a7c15UL), b = Mix(id ^ 0xd1b54a32d192ed03UL) | 1;
                
                for (int k = 0; k < bucket.Probes; k++)
                {
                    ulong bit = unchecked(a + (ulong)k * b) & mask;
                    bucket.Bits[(int)(bit >> 3)] |= (byte)(1 << (int)(bit & 7));
                }
            }

            m_buckets.Add(bucket);
            m_bytes += bytes + 16;
            m_ids.Clear();
        }

        internal ArchiveTimeBucketBloomIndex Complete()
        {
            Flush();
            return m_disabled ? null : new ArchiveTimeBucketBloomIndex((ulong)m_settings.BucketDurationSeconds * (ulong)TimeSpan.TicksPerSecond, [.. m_buckets]);
        }
    }

    internal static ArchiveTimeBucketBloomIndex Load(string path, Guid archive, uint sequence, Guid keyType, Guid valueType)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        try
        {
            using FileStream file = File.OpenRead(path + Extension);

            if (file.Length is < 32 or > MaximumBytes + 65536L)
                return null;

            byte[] expected = new byte[32];

            if (file.Read(expected, 0, 32) != 32)
                return null;

            byte[] payload;

            using (MemoryStream buffer = new())
            using (DeflateStream compressed = new(file, CompressionMode.Decompress))
            {
                byte[] chunk = new byte[8192];
                int read;

                while ((read = compressed.Read(chunk, 0, chunk.Length)) != 0)
                {
                    if (buffer.Length + read > MaximumBytes)
                        return null;

                    buffer.Write(chunk, 0, read);
                }

                payload = buffer.ToArray();
            }

            using (SHA256 hash = SHA256.Create())
            {
                if (!hash.ComputeHash(payload).SequenceEqual(expected))
                    return null;
            }

            using (BinaryReader reader = new(new MemoryStream(payload)))
            {
                if (reader.ReadInt32() != FileFormatSignature || new Guid(reader.ReadBytes(16)) != archive || reader.ReadUInt32() != sequence || new Guid(reader.ReadBytes(16)) != keyType || new Guid(reader.ReadBytes(16)) != valueType || reader.ReadInt64() != new FileInfo(path).Length)
                    return null;

                ulong width = reader.ReadUInt64();
                int count = reader.ReadInt32();

                if (width == 0 || count < 0 || count > MaximumBuckets || count > (payload.Length - 76) / 24)
                    return null;

                Bucket[] buckets = new Bucket[count];

                for (int i = 0; i < count; i++)
                {
                    ulong ordinal = reader.ReadUInt64();
                    int probes = reader.ReadInt32(), bytes = reader.ReadInt32();

                    if (probes < 1 || probes > 16 || bytes < 8 || (bytes & (bytes - 1)) != 0 || bytes > reader.BaseStream.Length - reader.BaseStream.Position || ordinal > ulong.MaxValue / width || (i > 0 && ordinal <= buckets[i - 1].Ordinal))
                        return null;

                    buckets[i] = new Bucket
                    {
                        Ordinal = ordinal,
                        Probes = probes,
                        Bits = reader.ReadBytes(bytes)
                    };
                }

                return reader.BaseStream.Position != reader.BaseStream.Length ? null : new ArchiveTimeBucketBloomIndex(width, buckets);
            }
        }
        catch (InvalidDataException)
        {

            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    internal void Save(string path, Guid archive, uint sequence, Guid keyType, Guid valueType)
    {
        string temporary = $"{path}{Extension}.{Guid.NewGuid():N}.tmp";

        try
        {
            byte[] payload;

            using (MemoryStream buffer = new())
            {
                using (BinaryWriter writer = new(buffer, System.Text.Encoding.UTF8, true))
                {
                    writer.Write(FileFormatSignature);
                    writer.Write(archive.ToByteArray());
                    writer.Write(sequence);
                    writer.Write(keyType.ToByteArray());
                    writer.Write(valueType.ToByteArray());
                    writer.Write(new FileInfo(path).Length);
                    writer.Write(m_width);
                    writer.Write(m_buckets.Length);

                    foreach (Bucket bucket in m_buckets)
                    {
                        writer.Write(bucket.Ordinal);
                        writer.Write(bucket.Probes);
                        writer.Write(bucket.Bits.Length);
                        writer.Write(bucket.Bits);
                    }
                }

                payload = buffer.ToArray();
            }

            using (FileStream file = new(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using (SHA256 hash = SHA256.Create())
                {
                    byte[] checksum = hash.ComputeHash(payload);
                    file.Write(checksum, 0, checksum.Length);
                }

                using (DeflateStream compressed = new(file, CompressionLevel.Optimal, true))
                {
                    compressed.Write(payload, 0, payload.Length);
                }

                file.Flush(true);
            }

            File.Move(temporary, path + Extension);
        }
        catch (IOException ex)
        {
            Logger.SwallowException(ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.SwallowException(ex);
        }
        finally
        {
            ArchivePointIDIndex.Delete(temporary, false);
        }
    }

    internal static void Delete(string path)
    {
        if (!string.IsNullOrEmpty(path)) 
            ArchivePointIDIndex.Delete(path + Extension, false);
    }

    internal static void Move(string oldPath, string newPath)
    {
        if (string.IsNullOrEmpty(oldPath) || oldPath == newPath)
            return;

        try
        {
            File.Move(oldPath + Extension, newPath + Extension);
        }
        catch (IOException ex)
        {
            Logger.SwallowException(ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.SwallowException(ex);
        }
    }
}