using System;
using System.Threading.Tasks;

namespace Venz.Images
{
    internal class UriPictureRequest: IRequest
    {
        private Int32 Id;
        private IImageUriRenderer Renderer;
        private UriPicture Picture;
        private Boolean IsValid => (Renderer.PictureRequestId != null) && Id.Equals(Renderer.PictureRequestId);

        public Object Tag => Renderer.Tag;



        public UriPictureRequest(IImageUriRenderer renderer, UriPicture picture)
        {
            Id = (Int32)DateTime.Now.Ticks;
            Renderer = renderer;
            Picture = picture;
            Renderer.PictureRequestId = Id;
        }

        public async Task<Boolean> ProcessAsync(TaskFactory imageRenderingTaskFactory)
        {
            var removeFromQueue = true;
            var pictureUri = Picture.UseAsyncPattern ? await Picture.GetUriAsync().ConfigureAwait(false) : Picture.GetUri();
            if (IsValid)
                await imageRenderingTaskFactory.StartNew(() => Renderer.UriContent = new ImageUriContent(Picture.Size, pictureUri)).ConfigureAwait(false);
            return removeFromQueue;
        }

        public Boolean IsRequestFor(Picture picture) => Picture == picture;
    }
}
