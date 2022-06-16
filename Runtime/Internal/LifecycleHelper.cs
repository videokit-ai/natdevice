/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System;
    using UnityEngine;

    internal sealed class LifecycleHelper : MonoBehaviour, IDisposable {
        
        #region --Client API--
        /// <summary>
        /// Event invoked when the app is paused or resumed.
        /// </summary>
        public event Action<bool> onPause;

        /// <summary>
        /// Event invoked when the app is exiting.
        /// </summary>
        public event Action onQuit;

        /// <summary>
        /// Create a lifecycle helper.
        /// </summary>
        public static LifecycleHelper Create () {
            var helper = new GameObject("NatDevice Lifecycle Helper").AddComponent<LifecycleHelper>();
            DontDestroyOnLoad(helper);
            return helper;
        }

        /// <summary>
        /// Dispose the helper.
        /// </summary>
        public void Dispose () {
            onPause = null;
            onQuit = null;
            GameObject.Destroy(gameObject);
        }
        #endregion


        #region --Operations--

        private void OnApplicationPause (bool pause) => onPause?.Invoke(pause);

        private void OnApplicationQuit () => onQuit?.Invoke();
        #endregion
    }
}