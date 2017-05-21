using System;
using System.Threading.Tasks;

namespace Venz.Images
{
    internal class StreamPictureRequest: IRequest
    {
        private Int32 Id;
        private IImageStreamRenderer Renderer;
        private StreamPicture Picture;
        private Boolean IsValid => (Renderer.PictureRequestId != null) && Id.Equals(Renderer.PictureRequestId);

        public Object Tag => Renderer.Tag;



        public StreamPictureRequest(IImageStreamRenderer renderer, StreamPicture picture)
        {
            Id = (Int32)DateTime.Now.Ticks;
            Renderer = renderer;
            Picture = picture;
            Renderer.PictureRequestId = Id;
        }

        public async Task<Boolean> ProcessAsync(TaskFactory imageRenderingTaskFactory)
        {
            var removeFromQueue = true;
            var pictureStream = Picture.UseAsyncPattern ? await Picture.GetStreamAsync().ConfigureAwait(false) : Picture.GetStream();
            if (IsValid)
                await imageRenderingTaskFactory.StartNew(() => Renderer.StreamContent = new ImageStreamContent(Picture.Size, pictureStream)).ConfigureAwait(false);
            return removeFromQueue;
        }

        public Boolean IsRequestFor(Picture picture) => Picture == picture;
    }
}
