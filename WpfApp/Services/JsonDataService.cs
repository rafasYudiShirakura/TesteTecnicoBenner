using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace WpfApp.Services
{
    public abstract class JsonDataService<T> : IDataService<T> where T : class
    {
        protected readonly string _filePath;
        private readonly string _idPropertyName;

        public JsonDataService(string fileName, string idPropertyName = "Id")
        {
            _filePath = Path.Combine("WpfApp", "Data", fileName);
            _idPropertyName = idPropertyName;
            EnsureDataFileExists();
        }

        private void EnsureDataFileExists()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        protected List<T> LoadData()
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar dados de {_filePath}: {ex.Message}");
                return new List<T>();
            }
        }

        protected void SaveData(List<T> data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<T> GetAll()
        {
            return LoadData();
        }

        public T GetById(int id)
        {
            return LoadData().AsQueryable().FirstOrDefault(e => (int)e.GetType().GetProperty(_idPropertyName).GetValue(e) == id);
        }

        public T Add(T entity)
        {
            var data = LoadData();
            
            var nextId = data.Any() ? data.AsQueryable().Max(e => (int)e.GetType().GetProperty(_idPropertyName).GetValue(e)) + 1 : 1;
            entity.GetType().GetProperty(_idPropertyName).SetValue(entity, nextId);

            data.Add(entity);
            SaveData(data);
            return entity;
        }

        public T Update(T entity)
        {
            var data = LoadData();
            var id = (int)entity.GetType().GetProperty(_idPropertyName).GetValue(entity);
            
            var existing = data.AsQueryable().FirstOrDefault(e => (int)e.GetType().GetProperty(_idPropertyName).GetValue(e) == id);
            
            if (existing != null)
            {
                var index = data.IndexOf(existing);
                data[index] = entity;
                SaveData(data);
                return entity;
            }
            throw new KeyNotFoundException($"Entidade com ID {id} não encontrada.");
        }

        public void Delete(int id)
        {
            var data = LoadData();
            
            var initialCount = data.Count;
            data.RemoveAll(e => (int)e.GetType().GetProperty(_idPropertyName).GetValue(e) == id);

            if (data.Count < initialCount)
            {
                SaveData(data);
            }
            else
            {
                throw new KeyNotFoundException($"Entidade com ID {id} não encontrada para exclusão.");
            }
        }
    }
}
