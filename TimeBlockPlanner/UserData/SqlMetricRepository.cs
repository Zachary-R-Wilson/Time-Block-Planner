﻿using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UserData.DataDelegates;
using UserData.Models;

namespace UserData
{
    public class SqlMetricRepository : IMetricRepository
    {
        private readonly SqlCommandExecutor executor;

        public SqlMetricRepository(string connectionString)
        {
            executor = new SqlCommandExecutor(connectionString);
        }

        public User CreateMetric(string name, int isDeleted)
        {
            // Verify parameters.
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The parameter cannot be null or empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(isDeleted.ToString()))
                throw new ArgumentException("The parameter cannot be null or empty.", nameof(isDeleted));

            // Save to database.
            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("User.CreateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("FirstName", firstName);
                        command.Parameters.AddWithValue("LastName", lastName);
                        command.Parameters.AddWithValue("Email", email);
                        command.Parameters.AddWithValue("Username", username);
                        command.Parameters.AddWithValue("PasswordHash", passwordHash);
                        command.Parameters.AddWithValue("IsDeleted", isDeleted);

                        var u = command.Parameters.Add("UserId", SqlDbType.Int);
                        u.Direction = ParameterDirection.Output;

                        connection.Open();

                        command.ExecuteNonQuery();

                        transaction.Complete();

                        var userId = (int)command.Parameters["UserId"].Value;

                        return new User(userId, firstName, lastName, email, username, passwordHash, isDeleted);
                    }
                }
            }
        }

        public void SaveMetric(int metricId, string name, int isComplete)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (isComplete != 0 && isComplete != 1)
                throw new ArgumentNullException(nameof(isComplete));


            var d = new SaveMetricDataDelegate(metricId, name , isComplete);
            executor.ExecuteNonQuery(d);
        }

        public Metric RetrieveMetric(int metricId)
        {
            var d = new RetrieveMetricDataDelegate(metricId);
            return executor.ExecuteReader(d);
        }
    }
}

