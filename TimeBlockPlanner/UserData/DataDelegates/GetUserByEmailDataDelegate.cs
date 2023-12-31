﻿using DataAccess;
using UserData.Models;
using System.Data.SqlClient;

namespace UserData.DataDelegates
{
    internal class GetUserByEmailDataDelegate : DataReaderDelegate<User>
    {
        private readonly string email;

        public GetUserByEmailDataDelegate(string email)
           : base("User.GetUserByEmail")
        {
            this.email = email;
        }

        public override void PrepareCommand(Command command)
        {
            base.PrepareCommand(command);

            command.Parameters.AddWithValue("Email", email);
        }

        public override User Translate(Command command, IDataRowReader reader)
        {
            if (!reader.Read())
                return null;

            return new User(
               reader.GetInt32("UserId"),
               reader.GetString("Username"),
               email,
               reader.GetString("FirstName"),
               reader.GetString("LastName"),
               reader.GetString("PasswordHash"),
               reader.GetInt32("IsDeleted"));
        }
    }
}