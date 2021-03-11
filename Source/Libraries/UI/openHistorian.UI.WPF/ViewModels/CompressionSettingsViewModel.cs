//******************************************************************************************************
//  CompressionSettingsViewModel.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  07/26/2017 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF.TimeSeries.UI;
using openHistorian.UI.DataModels;

namespace openHistorian.UI.WPF.ViewModels
{
    public class CompressionSettingsViewModel : PagedViewModelBase<CompressionSetting, int>
    {
        #region [ Constructors ]

        public CompressionSettingsViewModel()
            : base(25, false)
        {
        }

        #endregion

        #region [ Properties ]

        public override bool IsNewRecord => CurrentItem.IsNew;

    #endregion

        #region [ Methods ]

        public override int GetCurrentItemKey()
        {
            return CurrentItem.PointID;
        }

        public override string GetCurrentItemName()
        {
            return CurrentItem.PointTag;
        }

        #endregion

        #region [ Operators ]

        #endregion

        #region [ Static ]

        // Static Fields

        // Static Constructor

        // Static Properties

        // Static Methods

        #endregion
    }
}
