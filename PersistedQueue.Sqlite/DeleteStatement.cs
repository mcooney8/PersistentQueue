﻿using System;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    internal class DeleteStatement : IDisposable
    {
        private static readonly ParameterConverter<DatabaseItem> TypeMap =
            ParameterConverter.Builder<DatabaseItem>()
            .Ignore(dbItem => dbItem.SerializedItem)
            .Compile();

        private readonly Statement<DatabaseItem> statement;

        public DeleteStatement(string tableName, Connection connection)
        {
            string Sql = $@"
                delete from {tableName}
                where {nameof(DatabaseItem.Key)} = @{nameof(DatabaseItem.Key)}";
            statement = connection.CompileStatement(Sql, TypeMap);
        }

        public void Execute(uint key)
        {
            DatabaseItem item = new DatabaseItem
            {
                Key = key
            };
            statement.Bind(item);
            statement.Execute();
        }

        public void Dispose() => statement.Dispose();
    }
}
