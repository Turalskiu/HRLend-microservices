using AuthorizationApi.Models;

namespace AuthorizationApi.Repository
{
    public interface IObjectStoreRepository
    {
        public void InsertImg(string file, byte[] img);
        public void DeleteImg(string file);
    }

    public class FolderRepository : IObjectStoreRepository
    {
        private readonly string _path;

        public FolderRepository(string path)
        {
            _path = path;
        }


        public void InsertImg(string file, byte[] img)
        {
            string path = _path + "/Img/Photo/" + file;

            try
            {
                File.WriteAllBytes(path, img);
            }
            catch (IOException e)
            {
                Console.WriteLine("Произошла ошибка при удалении файла: " + e.Message);
            }
        }


        public void DeleteImg(string file) 
        {
            string path = _path + "/Img/Photo/" + file;

            try
            {
                // Проверяем существует ли файл
                if (File.Exists(path))
                {
                    // Удаляем файл
                    File.Delete(path);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Произошла ошибка при удалении файла: " + e.Message);
            }
        }

    }
}
