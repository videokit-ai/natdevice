/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// Camera device output that streams camera images into a `Texture2D`.
    /// The texture output uses the `PixelBufferOutput` to convert camera images to `RGBA8888` before uploading to the GPU.
    /// The rendered texture data is accessible on the CPU using the `Texture2D` data access methods.
    /// </summary>
    public sealed class TextureOutput : CameraOutput {

        #region --Client API--
        /// <summary>
        /// Texture conversion options.
        /// </summary>
        public class ConversionOptions : PixelBufferOutput.ConversionOptions { }

        /// <summary>
        /// Texture containing the latest camera image.
        /// </summary>
        public Texture2D texture { get; private set; }

        /// <summary>
        /// Texture timestamp.
        /// This is the timestamp of the image in the texture.
        /// </summary>
        public long timestamp { get; private set; }

        /// <summary>
        /// Get or set the texture orientation.
        /// </summary>
        public ScreenOrientation orientation {
            get => pixelBufferOutput.orientation;
            set => pixelBufferOutput.orientation = value;
        }

        /// <summary>
        /// Task that completes when the output texture has been created from the first camera image.
        /// This is equivalent to using `yield return new WaitUntil(() => output.texture)` in a coroutine.
        /// </summary>
        public Task textureCreated => taskCompletionSource.Task;

        /// <summary>
        /// Event raised when a new camera image is available in the texture output.
        /// </summary>
        public event Action onFrame;

        /// <summary>
        /// Create a texture output.
        /// </summary>
        public TextureOutput () {
            this.pixelBufferOutput = new PixelBufferOutput();
            this.taskCompletionSource = new TaskCompletionSource<Texture2D>();
            this.fence = new object();
            pixelBufferOutput.lifecycleHelper.onUpdate += OnFrame;
        }

        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="image">Camera image.</param>
        public override void Update (CameraImage image) => Update(image, null);

        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="image">Camera image.</param>
        /// <param name="options">Conversion options.</param>
        public void Update (CameraImage image, ConversionOptions options) {
            lock (fence)
                pixelBufferOutput.Update(image, options);
        }

        /// <summary>
        /// Dispose the texture output and release resources.
        /// </summary>
        public override void Dispose () {
            lock (fence) {
                pixelBufferOutput.Dispose();
                taskCompletionSource.TrySetCanceled();
                Texture2D.Destroy(texture);
            }
        }
        #endregion


        #region --Operations--
        private readonly PixelBufferOutput pixelBufferOutput;
        private readonly TaskCompletionSource<Texture2D> taskCompletionSource;
        private readonly object fence;

        private void OnFrame () {
            lock (fence) {
                // Check first frame
                if (pixelBufferOutput.timestamp == 0L)
                    return;
                // Check dirty
                if (timestamp == pixelBufferOutput.timestamp)
                    return;
                // Check texture
                var (width, height) = (pixelBufferOutput.width, pixelBufferOutput.height);
                texture = texture ? texture : new Texture2D(width, height, TextureFormat.RGBA32, false, false);
                if (texture.width != width || texture.height != height)
                    texture.Reinitialize(pixelBufferOutput.width, pixelBufferOutput.height);
                // Upload
                texture.GetRawTextureData<byte>().CopyFrom(pixelBufferOutput.pixelBuffer);
                texture.Apply();
                // Update timestamp
                timestamp = pixelBufferOutput.timestamp;
            }
            // Notify
            taskCompletionSource.TrySetResult(texture);
            onFrame?.Invoke();
        }

        [Obsolete(@"Deprecated in NatDevice 1.3.1. Use `textureCreated` property instead.", false)]
        public TaskAwaiter<Texture2D> GetAwaiter () => taskCompletionSource.Task.GetAwaiter();
        #endregion
    }
}