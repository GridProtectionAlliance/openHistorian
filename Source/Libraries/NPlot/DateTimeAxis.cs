/*
 * NPlot - A charting library for .NET
 * 
 * DateTimeAxis.cs
 * Copyright (C) 2003-2006 Matt Howlett and others.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 3. Neither the name of NPlot nor the names of its contributors may
 *    be used to endorse or promote products derived from this software without
 *    specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Drawing;
using System.Collections;
using System.Globalization;

// TODO: More control over how labels are displayed.
// TODO: SkipWeekends property.
// TODO: Make a relative (as opposed to absolute) TimeAxis.

namespace NPlot
{
	/// <summary>
	/// The DateTimeAxis class
	/// </summary>
	public class DateTimeAxis : Axis
	{

		#region Clone implementation
		/// <summary>
		/// Deep copy of DateTimeAxis.
		/// </summary>
		/// <returns>A copy of the DateTimeAxis Class.</returns>
		public override object Clone()
		{
			DateTimeAxis a = new DateTimeAxis();
			// ensure that this isn't being called on a derived type. If it is, then oh no!
			if (this.GetType() != a.GetType())
			{
				throw new NPlotException( "Clone not defined in derived type. Help!" );
			}
			DoClone( this, a );
			return a;
		}


		/// <summary>
		/// Helper method for Clone.
		/// </summary>
		/// <param name="a">The original object to clone.</param>
		/// <param name="b">The cloned object.</param>
		protected static void DoClone( DateTimeAxis b, DateTimeAxis a )
		{
			Axis.DoClone( b, a );
		}
		#endregion

		private void Init()
		{
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="a">Axis to construct from</param>
		public DateTimeAxis( Axis a )
			: base( a )
		{
			this.Init();
			this.NumberFormat = null;
		}


		/// <summary>
		/// Default Constructor
		/// </summary>
		public DateTimeAxis()
			: base()
		{
			this.Init();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="worldMin">World min of axis</param>
		/// <param name="worldMax">World max of axis</param>
		public DateTimeAxis( double worldMin, double worldMax )
			: base( worldMin, worldMax )
		{
			this.Init();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="worldMin">World min of axis</param>
		/// <param name="worldMax">World max of axis</param>
		public DateTimeAxis( long worldMin, long worldMax )
			: base( (double)worldMin, (double)worldMax )
		{
			this.Init();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="worldMin">World min of axis</param>
		/// <param name="worldMax">World max of axis</param>
		public DateTimeAxis( DateTime worldMin, DateTime worldMax )
			: base( (double)worldMin.Ticks, (double)worldMax.Ticks )
		{
			this.Init();
		}


		/// <summary>
		/// Draw the ticks.
		/// </summary>
		/// <param name="g">The drawing surface on which to draw.</param>
		/// <param name="physicalMin">The minimum physical extent of the axis.</param>
		/// <param name="physicalMax">The maximum physical extent of the axis.</param>
		/// <param name="boundingBox">out: smallest box that completely encompasses all of the ticks and tick labels.</param>
		/// <param name="labelOffset">out: a suitable offset from the axis to draw the axis label.</param>
		protected override void DrawTicks( 
			Graphics g, 
			Point physicalMin, 
			Point physicalMax, 
			out object labelOffset,
			out object boundingBox )
		{
			
			// TODO: Look at offset and bounding box logic again here. why temp and other vars? 

			Point tLabelOffset;
			Rectangle tBoundingBox;

			labelOffset = this.getDefaultLabelOffset( physicalMin, physicalMax );
			boundingBox = null;
		
			ArrayList largeTicks;
			ArrayList smallTicks;
			this.WorldTickPositions( physicalMin, physicalMax, out largeTicks, out smallTicks );

			// draw small ticks.
			for (int i=0; i<smallTicks.Count; ++i)
			{
				this.DrawTick( g, (double)smallTicks[i], 
					this.SmallTickSize, "", new Point(0, 0),
					physicalMin, physicalMax, 
					out tLabelOffset, out tBoundingBox );
				// assume label offset and bounding box unchanged by small tick bounds.
			}

			// draw large ticks.
			for (int i=0; i<largeTicks.Count; ++i)
			{
					
				DateTime tickDate = new DateTime( (long)((double)largeTicks[i]) );
                string label = LargeTickLabel(tickDate);

                this.DrawTick( g, (double)largeTicks[i],
	                            this.LargeTickSize, label, new Point( 0, 0 ),
                                physicalMin, physicalMax, out tLabelOffset, out tBoundingBox );

                Axis.UpdateOffsetAndBounds( ref labelOffset, ref boundingBox, tLabelOffset, tBoundingBox );
             }

        }

		/// <summary>
		/// Get the label corresponding to the provided date time
		/// </summary>
		/// <param name="tickDate">the date time to get the label for</param>
		/// <returns>label for the provided DateTime</returns>
		protected virtual string LargeTickLabel(DateTime tickDate)
		{
			string label = "";

			if(this.NumberFormat == null || this.NumberFormat == String.Empty) 
			{
				if ( this.LargeTickLabelType_ == LargeTickLabelType.year )
				{
					label = tickDate.Year.ToString();
				}

				else if ( this.LargeTickLabelType_ == LargeTickLabelType.month )
				{
					label = tickDate.ToString("MMM");
					label += " ";
					label += tickDate.Year.ToString().Substring(2,2);
				}

				else if ( this.LargeTickLabelType_ == LargeTickLabelType.day )
				{
					label = (tickDate.Day).ToString();
					label += " ";
					label += tickDate.ToString("MMM");
				}
				
				else if ( this.LargeTickLabelType_ == LargeTickLabelType.hourMinute )
				{
					string minutes = tickDate.Minute.ToString();
					if (minutes.Length == 1)
					{
						minutes = "0" + minutes;
					}
					label = tickDate.Hour.ToString() + ":" + minutes;
				}
				else if ( this.LargeTickLabelType_ == LargeTickLabelType.hourMinuteSeconds )
				{
					string minutes = tickDate.Minute.ToString();
					string seconds = tickDate.Second.ToString();
					if (seconds.Length == 1)
					{
						seconds = "0" + seconds;
					}

					if (minutes.Length == 1)
					{
						minutes = "0" + minutes;
					}
					label = tickDate.Hour.ToString() + ":" + minutes + "." + seconds;	
				}

			}
			else 
			{
				label = tickDate.ToString(NumberFormat);
			}

			return label;
		}


		/// <summary>
		/// Enumerates the different types of tick label possible.
		/// </summary>
		protected enum LargeTickLabelType 
		{
			/// <summary>
			/// default - no tick labels.
			/// </summary>
			none = 0,

			/// <summary>
			/// tick labels should be years
			/// </summary>
			year = 1,

			/// <summary>
			/// Tick labels should be month names
			/// </summary>
			month = 2,

			/// <summary>
			/// Tick labels should be day names
			/// </summary>
			day = 3,

			/// <summary>
			/// Tick labels should be hour / minutes.
			/// </summary>
			hourMinute = 4,

			/// <summary>
			/// tick labels should be hour / minute / second.
			/// </summary>
			hourMinuteSeconds = 5
		}


		/// <summary>
		///  this gets set after a get LargeTickPositions.
		/// </summary>
		protected LargeTickLabelType LargeTickLabelType_;


		/// <summary>
		/// Determines the positions, in world coordinates, of the large ticks. No
		/// small tick marks are currently calculated by this method.
		/// 
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">ArrayList containing the positions of the large ticks.</param>
		/// <param name="smallTickPositions">null</param>
		internal override void WorldTickPositions_FirstPass(
			Point physicalMin, 
			Point physicalMax,
			out ArrayList largeTickPositions,
			out ArrayList smallTickPositions
			)
		{
			smallTickPositions = null;

			largeTickPositions = new ArrayList();

			const int daysInMonth = 30;

			TimeSpan timeLength = new TimeSpan( (long)(WorldMax-WorldMin));
			DateTime worldMinDate = new DateTime( (long)this.WorldMin );
			DateTime worldMaxDate = new DateTime( (long)this.WorldMax );

			if(largeTickStep_ == TimeSpan.Zero) 
			{

				// if less than 10 minutes, then large ticks on second spacings. 

				if ( timeLength < new TimeSpan(0,0,2,0,0) )
				{
					this.LargeTickLabelType_ = LargeTickLabelType.hourMinuteSeconds;

					double secondsSkip;

					if (timeLength < new TimeSpan( 0,0,0,10,0 ) )
						secondsSkip = 1.0;
					else if ( timeLength < new TimeSpan(0,0,0,20,0) )
						secondsSkip = 2.0;
					else if ( timeLength < new TimeSpan(0,0,0,50,0) )
						secondsSkip = 5.0;
					else if ( timeLength < new TimeSpan(0,0,2,30,0) )
						secondsSkip = 15.0;
					else 
						secondsSkip = 30.0;

					int second = worldMinDate.Second;
					second -= second % (int)secondsSkip;					

					DateTime currentTickDate = new DateTime( 
						worldMinDate.Year,
						worldMinDate.Month, 
						worldMinDate.Day,
						worldMinDate.Hour,
						worldMinDate.Minute,
						second,0 );

					while ( currentTickDate < worldMaxDate )
					{
						double world = (double)currentTickDate.Ticks;

						if ( world >= this.WorldMin && world <= this.WorldMax )
						{
							largeTickPositions.Add( world );
						}

						currentTickDate = currentTickDate.AddSeconds( secondsSkip );
					}
				}

				// Less than 2 hours, then large ticks on minute spacings.

				else if ( timeLength < new TimeSpan(0,2,0,0,0) )
				{
					this.LargeTickLabelType_ = LargeTickLabelType.hourMinute;

					double minuteSkip;

					if ( timeLength < new TimeSpan(0,0,10,0,0) )
						minuteSkip = 1.0;
					else if ( timeLength < new TimeSpan(0,0,20,0,0) )
						minuteSkip = 2.0;
					else if ( timeLength < new TimeSpan(0,0,50,0,0) )
						minuteSkip = 5.0;
					else if ( timeLength < new TimeSpan(0,2,30,0,0) )
						minuteSkip = 15.0;
					else //( timeLength < new TimeSpan( 0,5,0,0,0) )
						minuteSkip = 30.0;

					int minute = worldMinDate.Minute;
					minute -= minute % (int)minuteSkip;					

					DateTime currentTickDate = new DateTime( 
						worldMinDate.Year,
						worldMinDate.Month, 
						worldMinDate.Day,
						worldMinDate.Hour,
						minute,0,0 );

					while ( currentTickDate < worldMaxDate )
					{
						double world = (double)currentTickDate.Ticks;

						if ( world >= this.WorldMin && world <= this.WorldMax )
						{
							largeTickPositions.Add( world );
						}

						currentTickDate = currentTickDate.AddMinutes( minuteSkip );
					}
				}

				// Less than 2 days, then large ticks on hour spacings.

				else if ( timeLength < new TimeSpan(2,0,0,0,0) )
				{
					this.LargeTickLabelType_ = LargeTickLabelType.hourMinute;

					double hourSkip;
					if ( timeLength < new TimeSpan(0,10,0,0,0) )
						hourSkip = 1.0;
					else if ( timeLength < new TimeSpan(0,20,0,0,0) )
						hourSkip = 2.0;
					else
						hourSkip = 6.0;


					int hour = worldMinDate.Hour;
					hour -= hour % (int)hourSkip;					

					DateTime currentTickDate = new DateTime( 
						worldMinDate.Year,
						worldMinDate.Month, 
						worldMinDate.Day,
						hour,0,0,0 );

					while ( currentTickDate < worldMaxDate )
					{
						double world = (double)currentTickDate.Ticks;

						if ( world >= this.WorldMin && world <= this.WorldMax )
						{
							largeTickPositions.Add( world );
						}

						currentTickDate = currentTickDate.AddHours( hourSkip );
					}

				}


				// less than 5 months, then large ticks on day spacings.

				else if ( timeLength < new TimeSpan(daysInMonth*4,0,0,0,0))
				{
					this.LargeTickLabelType_ = LargeTickLabelType.day;

					double daySkip;
					if ( timeLength < new TimeSpan(10,0,0,0,0) )
						daySkip = 1.0;
					else if (timeLength < new TimeSpan(20,0,0,0,0) )
						daySkip = 2.0;
					else if (timeLength < new TimeSpan(7*10,0,0,0,0) )
						daySkip = 7.0;
					else 
						daySkip = 14.0;

					DateTime currentTickDate = new DateTime( 
						worldMinDate.Year,
						worldMinDate.Month, 
						worldMinDate.Day );

                    if (daySkip == 2.0)
                    {

                        TimeSpan timeSinceBeginning = currentTickDate - DateTime.MinValue;

                        if (timeSinceBeginning.Days % 2 == 1)
                            currentTickDate = currentTickDate.AddDays(-1.0);
                    }

                    if (daySkip == 7 || daySkip == 14.0)
                    {
                        DayOfWeek dow = currentTickDate.DayOfWeek;
                        switch (dow)
                        {
                            case DayOfWeek.Monday:
                                break;
                            case DayOfWeek.Tuesday:
                                currentTickDate = currentTickDate.AddDays(-1.0);
                                break;
                            case DayOfWeek.Wednesday:
                                currentTickDate = currentTickDate.AddDays(-2.0);
                                break;
                            case DayOfWeek.Thursday:
                                currentTickDate = currentTickDate.AddDays(-3.0);
                                break;
                            case DayOfWeek.Friday:
                                currentTickDate = currentTickDate.AddDays(-4.0);
                                break;
                            case DayOfWeek.Saturday:
                                currentTickDate = currentTickDate.AddDays(-5.0);
                                break;
                            case DayOfWeek.Sunday:
                                currentTickDate = currentTickDate.AddDays(-6.0);
                                break;
                        }

                    }

                    if (daySkip == 14.0f)
                    {
                        TimeSpan timeSinceBeginning = currentTickDate - DateTime.MinValue;

                        if ((timeSinceBeginning.Days / 7) % 2 == 1)
                        {
                            currentTickDate = currentTickDate.AddDays(-7.0);
                        }
                    }

                    while ( currentTickDate < worldMaxDate )
					{
						double world = (double)currentTickDate.Ticks;

						if ( world >= this.WorldMin && world <= this.WorldMax )
						{
							largeTickPositions.Add( world );
						}

						currentTickDate = currentTickDate.AddDays(daySkip);
					}
				}


					// else ticks on month or year spacings.

				else if ( timeLength >= new TimeSpan(daysInMonth*4,0,0,0,0) )
				{

					int monthSpacing = 0;
			
					if ( timeLength.Days < daysInMonth*(12*3+6) )
					{
						LargeTickLabelType_ = LargeTickLabelType.month;

						if ( timeLength.Days < daysInMonth*10 )
							monthSpacing = 1;
						else if ( timeLength.Days < daysInMonth*(12*2) )
							monthSpacing = 3;
						else // if ( timeLength.Days < daysInMonth*(12*3+6) )
							monthSpacing = 6;
					}
					else
					{
						LargeTickLabelType_ = LargeTickLabelType.year;

                        if (timeLength.Days < daysInMonth * (12 * 6))
                            monthSpacing = 12;
                        else if (timeLength.Days < daysInMonth * (12 * 12))
                            monthSpacing = 24;
                        else if (timeLength.Days < daysInMonth * (12 * 30))
                            monthSpacing = 60;
                        else
                            monthSpacing = 120;
                        //LargeTickLabelType_ = LargeTickLabelType.none;
					}

					// truncate start
					DateTime currentTickDate = new DateTime( 
						worldMinDate.Year,
						worldMinDate.Month, 
						1 );
			
					if (monthSpacing > 1)
					{
						currentTickDate = currentTickDate.AddMonths(
							-(currentTickDate.Month-1)%monthSpacing );
					}

					// Align on 2 or 5 year boundaries if necessary.
					if (monthSpacing >= 24)
					{
						currentTickDate = currentTickDate.AddYears(
							-(currentTickDate.Year)%(monthSpacing/12) );						
					}

					//this.firstLargeTick_ = (double)currentTickDate.Ticks;

					if ( LargeTickLabelType_ != LargeTickLabelType.none )
					{
						while ( currentTickDate < worldMaxDate )
						{
							double world = (double)currentTickDate.Ticks;

							if ( world >= this.WorldMin && world <= this.WorldMax )
							{
								largeTickPositions.Add( world );
							}

							currentTickDate = currentTickDate.AddMonths( monthSpacing );
						}
					}
				}
			}
			else
			{
				for (DateTime date = worldMinDate; date < worldMaxDate; date += largeTickStep_) 
				{
					largeTickPositions.Add((double)date.Ticks);
				}
			}
        }


        /// <summary>
        /// Compute the small tick positions for largetick size of one or more years.
        ///  - inside the domain or the large tick positons, is take the mid-point of pairs of large ticks
        ///  - outside the large tick range, check if a half tick is inside the world min/max
        /// This method works only if there are atleast 2 large ticks,
        /// since we don't know if its minutes, hours, month, or yearly divisor.
        /// </summary>
        /// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
        /// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
        /// <param name="largeTickPositions">Read in the large tick positions</param>
        /// <param name="smallTickPositions">Fill in the corresponding small tick positions</param>
        /// <remarks>Added by Rosco Hill</remarks>
        internal override void WorldTickPositions_SecondPass(
            Point physicalMin,
            Point physicalMax,
            ArrayList largeTickPositions,
            ref ArrayList smallTickPositions
          )
        {
            if (largeTickPositions.Count < 2 || !(LargeTickLabelType_.Equals(LargeTickLabelType.year)))
            {
                smallTickPositions = new ArrayList(); ;
            }
            else
            {
                smallTickPositions = new ArrayList();
                double diff = 0.5 * (((double)largeTickPositions[1]) - ((double)largeTickPositions[0]));
                if (((double)largeTickPositions[0] - diff) > this.WorldMin)
                {
                    smallTickPositions.Add((double)largeTickPositions[0] - diff);
                }
                for (int i = 0; i < largeTickPositions.Count - 1; i++)
                {
                    smallTickPositions.Add(((double)largeTickPositions[i]) + diff);
                }
                if (((double)largeTickPositions[largeTickPositions.Count - 1] + diff) < this.WorldMax)
                {
                    smallTickPositions.Add((double)largeTickPositions[largeTickPositions.Count - 1] + diff);
                }
            }
        }


        /// <summary>
		/// The distance between large ticks. If this is set to Zero [default],
		/// this distance will be calculated automatically.
		/// </summary>
		public TimeSpan LargeTickStep 
		{
			set 
			{
				largeTickStep_ = value;
			}
			get 
			{
				return largeTickStep_;
			}
		}
 		private TimeSpan largeTickStep_ = TimeSpan.Zero;


	}
}
