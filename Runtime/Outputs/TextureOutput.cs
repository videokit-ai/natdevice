/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

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
        /// Get or set the texture orientation.
        /// </summary>
        public ScreenOrientation orientation {
            get => pixelBufferOutput.orientation;
            set => pixelBufferOutput.orientation = value;
        }

        /// <summary>
        /// Create a texture output.
        /// </summary>
        public TextureOutput () {
            this.pixelBufferOutput = new PixelBufferOutput();
            this.taskCompletionSource = new TaskCompletionSource<Texture2D>();
        }

        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="cameraImage">Camera image.</param>
        /// <param name="options">Conversion options.</param>
        public override void Update (CameraImage cameraImage) => Update(cameraImage, null);

        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="cameraImage">Camera image.</param>
        /// <param name="options">Conversion options.</param>
        public void Update (CameraImage cameraImage, ConversionOptions options) {
            pixelBufferOutput.Update(cameraImage, options);
            var (width, height) = (pixelBufferOutput.width, pixelBufferOutput.height);
            texture = texture ? texture : new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            if (texture.width != width || texture.height != height)
                texture.Resize(pixelBufferOutput.width, pixelBufferOutput.height);
            texture.GetRawTextureData<byte>().CopyFrom(pixelBufferOutput.pixelBuffer);
            texture.Apply();
            taskCompletionSource.TrySetResult(texture);
        }

        /// <summary>
        /// Dispose the texture output and release resources.
        /// </summary>
        public override void Dispose () {
            pixelBufferOutput.Dispose();
            taskCompletionSource.TrySetCanceled();
            Texture2D.Destroy(texture);
        }
        #endregion


        #region --Operations--
        private readonly PixelBufferOutput pixelBufferOutput;
        private readonly TaskCompletionSource<Texture2D> taskCompletionSource;

        public TaskAwaiter<Texture2D> GetAwaiter () => taskCompletionSource.Task.GetAwaiter();
        #endregion
    }
}