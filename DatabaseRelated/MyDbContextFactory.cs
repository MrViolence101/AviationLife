using core.DatabaseRelated.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySQL.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.DatabaseRelated
{
    class MyDbContextFactory : IDbContextFactory<MyDbContext>
    {
        public MyDbContext Create(DbContextFactoryOptions options)
        {
            const string db = "Server=localhost;Port=27501;Database=test;User ID=root;Password=mypassword;";

            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            optionsBuilder.UseMySQL(db);

            var context = new MyDbContext(optionsBuilder.Options);

            try
            {
                context.Database.ExecuteSqlCommand("CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` ( `MigrationId` nvarchar(150) NOT NULL, `ProductVersion` nvarchar(32) NOT NULL, PRIMARY KEY (`MigrationId`) );"); // table is not created by the MySql package :/
                
                context.Database.Migrate();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new MyDbContext(optionsBuilder.Options);
            }


            return context;
        }
    }
}
