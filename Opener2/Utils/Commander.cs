using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Opener2.Models;

namespace Opener2.Utils
{
	public class Commander
	{
		// Connection strings
		private readonly string _webConnection = ConfigurationManager.ConnectionStrings["ESP_WEB"].ConnectionString;

		public bool operationSuccess;

		#region Agreements
		/// <summary>
		/// Получить информацию о договоре.
		/// </summary>
		/// <param name="snils"></param>
		/// <returns></returns>
		public Agreement Check(string snils)
		{
			const string sqlExpression = @"select * from AgreementsView2 where snils = (@snils)";

			var a = new Agreement();
			using (var connection = new SqlConnection(_webConnection))
			{
				connection.Open();
				var cmd = new SqlCommand(sqlExpression, connection);
				var snilsParam = new SqlParameter("@snils", snils);
				cmd.Parameters.Add(snilsParam);
				var reader = cmd.ExecuteReader();

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						a.CreatedOn = reader.GetValue(0).ToString();
						a.Npf = reader.GetValue(1).ToString();
						a.Snils = reader.GetValue(2).ToString();
						a.ContactName = reader.GetValue(3).ToString();
						a.Gender = reader.GetValue(4).ToString();
						a.BirthDate = reader.GetValue(5).ToString();
						a.Status = reader.GetValue(6).ToString();
						a.Comment = reader.GetValue(7).ToString();
						a.AgentUsername = reader.GetValue(8).ToString();
						a.Agent = reader.GetValue(9).ToString();
						a.Signer = reader.GetValue(10).ToString();
						a.BusinessUnit = reader.GetValue(11).ToString();
						a.SaleChannel = reader.GetValue(12).ToString();
					}
					reader.Close();
				}
				else
				{
					throw new Exception("Нет такого договора");
				}
				return a;
			}
		}

		/// <summary>
		/// Обновить дату создания договора.
		/// </summary>
		/// <param name="snils"></param>
		public void UpdateCreatedDate(string snils)
		{
			operationSuccess = false;
			const string sqlExpression = @"UPDATE Agreements set CreatedOn = GetDate()
											WHERE snils = (@snils)";

			using (var connection = new SqlConnection(_webConnection))
			{
				connection.Open();
				var cmd = new SqlCommand(sqlExpression, connection);
				var snilsParam = new SqlParameter("@snils", snils);
				cmd.Parameters.Add(snilsParam);
				int returned = cmd.ExecuteNonQuery();
				if (returned == 1)
				{
					operationSuccess = true;
				}
				else
				{
					operationSuccess = false;
					throw new Exception("Что-то пошло не так..");
				}
			}
		}

		/// <summary>
		/// Открыть договор для редактирования, сбросить все статусы.
		/// </summary>
		/// <param name="snils"></param>
		public void OpenAgreement(string snils)
		{
			operationSuccess = false;
			const string sqlExpression = @" UPDATE Agreements SET 
											   Status_Web = 200000001,
											   Partner_Id = null, --чистый id 
											   Partner_Uid = null, --чистый uid
											   Partner_JSON = null,
											   Status_WebDate = getdate() --дата веб статуса текущая
											WHERE snils = (@snils);";

			using (var connection = new SqlConnection(_webConnection))
			{
				connection.Open();
				var cmd = new SqlCommand(sqlExpression, connection);
				var snilsParam = new SqlParameter("@snils", snils);
				cmd.Parameters.Add(snilsParam);
				int returned = cmd.ExecuteNonQuery();
				if (returned == 1)
				{
					operationSuccess = true;
				}
				else
				{
					throw new Exception("Что-то пошло не так..");
				}
			}
		}

		/// <summary>
		/// Сменить НПФ, сбросить все статусы.
		/// </summary>
		/// <param name="snils"></param>
		/// <param name="npf"></param>
		public void ChangeNpf(string snils, string npf)
		{
			operationSuccess = false;
			//TODO: Вытягивать имена фондов из базы
			string NPF_name = "";
			switch (npf)
			{
				case "gaz": NPF_name = "НПФ «Газфонд»"; break;
				case "saf": NPF_name = "НПФ «САФМАР»"; break;
				case "naz": NPF_name = "НПФ «Национальный»"; break;
			}

			const string sqlExpression = @"DECLARE @npfid uniqueIdentifier;
											set @npfid = (SELECT id FROM Npfs where Name = (@NPF_name));
											
											UPDATE Agreements SET 
											   NpfId = @npfid,
											   Status_Web = 200000001,
											   Partner_Id = null, --чистый id 
											   Partner_Uid = null, --чистый uid
											   Partner_Json = null,
											   Status_WebDate = getdate() --дата веб статуса текущая
											WHERE snils = (@snils);";

			using (var connection = new SqlConnection(_webConnection))
			{
				connection.Open();
				var cmd = new SqlCommand(sqlExpression, connection);
				var snilsParam = new SqlParameter("@snils", snils);
				var NPF_nameParam = new SqlParameter("@NPF_name", NPF_name);
				cmd.Parameters.Add(snilsParam);
				cmd.Parameters.Add(NPF_nameParam);
				int returned = cmd.ExecuteNonQuery();
				if (returned == 1)
				{
					operationSuccess = true;
				}
				else
				{
					throw new Exception("Что-то пошло не так..");
				}
			}
		}

		/// <summary>
		/// Сменить агента/создателя.
		/// </summary>
		/// <param name="snils"></param>
		/// <param name="username"></param>
		public void ChangeAgent(string snils, string username)
		{
			operationSuccess = false;

			const string sqlExpression = @"update a set CreatorId = (select id from Employees where username like (@username))
											from Agreements a 
											where a.snils = (@snils);";

			using (var connection = new SqlConnection(_webConnection))
			{
				connection.Open();
				var cmd = new SqlCommand(sqlExpression, connection);
				var snilsParam = new SqlParameter("@snils", snils);
				var usernameParam = new SqlParameter("@username", username);
				cmd.Parameters.Add(snilsParam);
				cmd.Parameters.Add(usernameParam);
				int returned = cmd.ExecuteNonQuery();
				if (returned == 1)
				{
					operationSuccess = true;
				}
				else
				{
					throw new Exception("Что-то пошло не так..");
				}
			}
		}

		//TODO: Добавить ChangeSigner

		/// <summary>
		/// Получить список договоров по фонду и промежуткам дат.
		/// </summary>
		/// <param name="npf"></param>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public List<Agreement> GetAgreements(string npf, DateTime d1, DateTime d2)
		{
			operationSuccess = false;

			string npfName = "";
			switch (npf)
			{
				case "gaz": npfName = "НПФ «Газфонд»"; break;
				case "saf": npfName = "НПФ «САФМАР»"; break;
				case "naz": npfName = "НПФ «Национальный»"; break;
			}

			d2 = d2.AddDays(1);

			const string sqlExpression = "SELECT * FROM AgreementsView2 " +
										 "WHERE Npf = (@npf) " +
										 "AND CreatedOn BETWEEN (@date1) AND (@date2) " +
										 "ORDER BY CreatedOn;";

			var agreementsList = new List<Agreement>();
			using (var connection = new SqlConnection(_webConnection))
			{
				connection.Open();
				var cmd = new SqlCommand(sqlExpression, connection);
				var date1Param = new SqlParameter("@date1", d1);
				var date2Param = new SqlParameter("@date2", d2);
				var npfParam = new SqlParameter("@npf", npfName);
				cmd.Parameters.Add(date1Param);
				cmd.Parameters.Add(date2Param);
				cmd.Parameters.Add(npfParam);
				var reader = cmd.ExecuteReader();

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						var a = new Agreement();
						var parseDate = new DateTime();
						DateTime.TryParse(reader.GetValue(0).ToString(), out parseDate);
						a.CreatedOn = parseDate.ToString("dd.MM.yyyy");
						a.Npf = reader.GetValue(1).ToString();
						a.Snils = reader.GetValue(2).ToString();
						a.ContactName = reader.GetValue(3).ToString();
						a.Gender = reader.GetValue(4).ToString();
						a.BirthDate = reader.GetValue(5).ToString();
						a.Status = reader.GetValue(6).ToString();
						a.Comment = reader.GetValue(7).ToString();
						a.AgentUsername = reader.GetValue(8).ToString();
						a.Agent = reader.GetValue(9).ToString();
						a.Signer = reader.GetValue(10).ToString();
						a.BusinessUnit = reader.GetValue(11).ToString();
						a.SaleChannel = reader.GetValue(12).ToString();

						agreementsList.Add(a);
					}
					reader.Close();
				}
				else
				{
					throw new Exception("Нет такого договора");
				}

				return agreementsList;
			}
		}

		/// <summary>
		/// Получить список договоров по фонду и промежуткам дат.
		/// </summary>
		/// <param name="fund"></param>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public async Task<List<Agreement>> GetAgreementsAsync(string fund, DateTime d1, DateTime d2)
		{
			return await Task.Run(() => GetAgreements(fund, d1, d2));
		}

		#endregion

		#region Related Products

		/// <summary>
		/// Получить список сторонних продуктов.
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public List<RelatedProduct> GetRelatedProducts(DateTime d1, DateTime d2)
        {
            operationSuccess = false;

            d2 = d2.AddDays(1);

            const string sqlExpression = "SELECT * FROM RelatedProductsView WHERE createdon BETWEEN (@date1) AND (@date2) ORDER BY CreatedOn;";

            var relatedProducts = new List<RelatedProduct>();
            using (var connection = new SqlConnection(_webConnection))
            {
                connection.Open();
                var cmd = new SqlCommand(sqlExpression, connection);
                var date1Param = new SqlParameter("@date1", d1);
                var date2Param = new SqlParameter("@date2", d2);
                cmd.Parameters.Add(date1Param);
                cmd.Parameters.Add(date2Param);
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var product = new RelatedProduct();
                        var parseDate = new DateTime();
                        DateTime.TryParse(reader.GetValue(0).ToString(), out parseDate);
                        product.Created = parseDate.ToString("dd.MM.yyyy");
                        if (reader.GetValue(1).GetType() != typeof(System.DBNull))
                        {
                            product.ExternalId = Convert.ToInt32(reader.GetValue(1));
                        }
                        else
                        {
                            product.ExternalId = null;
                        }
                        product.Agent = reader.GetValue(2).ToString();
                        product.SaleChannel = reader.GetValue(3).ToString();
                        product.BusinessUnit = reader.GetValue(4).ToString();
                        product.Product = reader.GetValue(5).ToString();
                        product.Mobilephone = Convert.ToInt64(reader.GetValue(6));
                        product.Lastname = reader.GetValue(7).ToString();
                        product.Firstname = reader.GetValue(8).ToString();
                        product.Middlename = reader.GetValue(9).ToString();

                        relatedProducts.Add(product);
                    }
                    reader.Close();
                }

                return relatedProducts;
            }
        }

		/// <summary>
		/// Получить список сторонних продуктов.
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public async Task<List<RelatedProduct>> GetRelatedProductsAsync(DateTime d1, DateTime d2)
        {
            return await Task.Run(() => GetRelatedProducts(d1, d2));
        }

		#endregion

        /// <summary>
		/// Получить список сотрудников по фамилии.
		/// </summary>
		/// <param name="lastname"></param>
		/// <param name="employeesList"></param>
		public void GetEmployee(string lastname, out List<string> employeesList)
		{
			employeesList = new List<string>();

			const string sqlExpression = @"declare @lastname varchar(20);
										  set @lastname = (@lastnameParam);
										  select * from EmployeesWithPasswordTable
										  where lastname like '%' + @lastname + '%'";

			using (var connection = new SqlConnection(_webConnection))
			{
                connection.Open();
                var cmd = new SqlCommand(sqlExpression, connection);
                var lastnameParam = new SqlParameter("@lastnameParam", lastname);
                cmd.Parameters.Add(lastnameParam);

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        employeesList.Add(reader.GetValue(1).ToString());
                        employeesList.Add(reader.GetValue(2).ToString());
                        employeesList.Add(reader.GetValue(3).ToString());
                        employeesList.Add(reader.GetValue(4).ToString());
                        employeesList.Add(reader.GetValue(5).ToString());
                    }
                    reader.Close();
                }
                else
                {
                    throw new Exception("Нет таких пользователей");
                }

            }
		}
    }
}
