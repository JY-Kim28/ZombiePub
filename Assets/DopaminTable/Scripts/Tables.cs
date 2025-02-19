using System;

namespace DopaminTable
{
    public class Tables
    {
        static Tables _Instance;

        int _TotalTableCount = 1;
        int _LoadTableCount;

        Action<int, int> _ProgressCallback;


        //TABLE LIST - START        Table_MachineLevel200 _MachineLevel200;
        public static Table_MachineLevel200 MachineLevel200 => _Instance._MachineLevel200;
        //TABLE LIST - END

        public static void Create()
        {
            _Instance ??= new Tables();
            _Instance._MachineLevel200 = new Table_MachineLevel200();
        }



        public static void Load(Action<int, int> progressCallback)
        {
            _Instance._ProgressCallback = progressCallback;
            _Instance._LoadTableCount = 0;


            _Instance._MachineLevel200.Load(_Instance.CompleteLoad);
        }

        private void CompleteLoad()
        {
            ++_LoadTableCount;

            _ProgressCallback?.Invoke(_LoadTableCount, _TotalTableCount);
        }
    }
}