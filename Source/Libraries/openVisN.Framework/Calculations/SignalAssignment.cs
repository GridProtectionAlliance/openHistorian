using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Data.Query;

namespace openVisN.Calculations
{

    class SignalAssignment : CalculationMethod
    {
        MetadataBase m_newSignal;

        public SignalAssignment()
            : base(new MetadataDouble(Guid.NewGuid(), null, "", ""))
        {
            m_newSignal = new MetadataDouble(Guid.NewGuid(), null, "", "", this);
        }

        public void GetPoints(out MetadataBase newSignal)
        {
            newSignal = m_newSignal;
        }

        public void SetNewBaseSignal(MetadataBase signalId)
        {
            Dependencies[0] = signalId;
        }

        public override void Calculate(IDictionary<Guid, SignalDataBase> signals)
        {
            Dependencies[0].Calculate(signals);

            var origionalSignal = signals[Dependencies[0].UniqueId];

            var newSignal = TryGetSignal(m_newSignal, signals);

            if (newSignal == null || newSignal.IsComplete)
                return;

            int pos = 0;

            while (pos < origionalSignal.Count)
            {
                ulong time;
                double vm;
                origionalSignal.GetData(pos, out time, out vm);
                pos++;

                newSignal.AddData(time, vm);
            }
            newSignal.Completed();
        }

        SignalDataBase TryGetSignal(MetadataBase signal, IDictionary<Guid, SignalDataBase> results)
        {
            SignalDataBase data;
            if (results.TryGetValue(signal.UniqueId, out data))
                return data;
            return null;
        }
    }
}
