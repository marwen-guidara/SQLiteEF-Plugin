using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace Root.Services.Sqlite
{
    public class DataBaseContext : DbContext
    {
        private readonly string _dataBasePath;

        private readonly List<Type> _listOfModels;

        public DataBaseContext(List<Type> listOfModels, string dataBaseName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dataBasePath = Path.Combine(path, $"{dataBaseName}.db3");
            _listOfModels = listOfModels;
            if (!File.Exists(_dataBasePath))
                Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_dataBasePath}");
            optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var item in _listOfModels)
            {
                modelBuilder.Entity(item).ToTable(item.Name);
            }
            base.OnModelCreating(modelBuilder);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void DeleteDataBase()
        {
            if (File.Exists(_dataBasePath))
                File.Delete(_dataBasePath);
        }

    }
}
