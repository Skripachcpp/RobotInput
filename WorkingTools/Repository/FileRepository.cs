using System;
using System.Collections.Generic;
using System.IO;
using WorkingTools.Extensions;

namespace WorkingTools.Repository
{
    public class FileRepository<TKey, TValue> : RepositoryAbstract<TKey, TValue>
    {
        public FileRepository(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentOutOfRangeException("filePath", "не указан путь до файла");

            FilePath = filePath;
        }

        public string FilePath { get; private set; }

        protected override IEnumerable<Pair<TKey, TValue>> LoadInems()
        {
            string filePath = FilePath;

            if (!File.Exists(filePath))
                return null;

            using (var fileStream = File.OpenRead(filePath))
            {
                Pair<TKey, TValue>[] repositoryItems = null;

                try { repositoryItems = fileStream.FromXml<Pair<TKey, TValue>[]>(); }
                catch (Exception ex) { }

                return repositoryItems;
            }
        }

        protected override void SaveItems(IEnumerable<KeyValuePair<TKey, TValue>> items, IEnumerable<KeyValuePair<TKey, TValue>> newItems, IEnumerable<KeyValuePair<TKey, TValue>> updItems, IEnumerable<KeyValuePair<TKey, TValue>> delItems)
        {
            string filePath = FilePath;

            var repositoryItems = new List<Pair<TKey, TValue>>();
            items.ForEach(item => repositoryItems.Add(new Pair<TKey, TValue>() { Key = item.Key, Value = item.Value }));

            if (repositoryItems.Count <= 0)
                File.Delete(filePath);
            else
                using (var memoryStream = repositoryItems.ToXml())
                    memoryStream.Save(filePath);
        }
    }

    public class Pair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}
