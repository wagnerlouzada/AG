using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CustomExtensions;
using Identity.Domain;
using Ag.Domain;

namespace Api.Models {


    public static class DB
    {

        private static readonly string connectionString = "Data Source=(localdb)\\mssqllocaldb;AttachDbFilename=c:\\WL\\AG_data.mdf;Integrated Security=True;Connect Timeout=600";
    
        private static readonly DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

        public static int Update(string sql)
        {
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetDataTable(string sql)
        {
           
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    using (DbDataAdapter adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = command;

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        return dt;
                    }
                }
            }
        }

    }

    public class AgContext:DbContext
    {

        public AgContext(DbContextOptions<AgContext> options) : base(options) { }
        //public AgContext(DbContextOptions<AgContext> options) : base(options) { }

        public List<AgContentView> GetContents()
        {

            //var data = this.AgContents.ToListAsync();
            //if (data == null)
            //{
            DataTable dt = DB.GetDataTable("SELECT * From Content");
                return MakeContent(dt);
            //}
            //return null;
        }

        public List<AgRightView> GetRights()
        {
            DataTable dt = DB.GetDataTable("SELECT * From Rights");
            return MakeRight(dt);
        }

        private List<AgContentView> MakeContent(DataTable dt)
        {
            List<AgContentView> list = new List<AgContentView>();
            foreach (DataRow row in dt.Rows) list.Add(MakeContent(row));
            return list;
        }

        private List<AgRightView> MakeRight(DataTable dt)
        {
            List<AgRightView> list = new List<AgRightView>();
            foreach (DataRow row in dt.Rows) list.Add(MakeRight(row));
            return list;
        }

        private AgRightView MakeRight(DataRow row)
        {

            AgRightView agRights = new AgRightView();

            agRights.ID = row["ID"].ObjectToLong();
            agRights.FkContent = row["FkContent"].ObjectToLong();
            agRights.FkUser = row["FkUser"].ObjectToLong();
            agRights.FkNotificationStatus = row["FkNotificationStatus"].ObjectToLong();
            agRights._DestinationRights_ = row["_DestinationRights_"].ObjectToBoolean();
            agRights.Create = row["Create"].ObjectToBoolean();
            agRights.Read = row["Read"].ObjectToBoolean();
            agRights.Update = row["Update"].ObjectToBoolean();
            agRights.Delete = row["Delete"].ObjectToBoolean();
            agRights.Write = row["Write"].ObjectToBoolean();
            agRights.Execute = row["Execute"].ObjectToBoolean();
            agRights.DelegateRights = row["DelegateRights"].ObjectToBoolean();
            agRights.Share = row["Share"].ObjectToBoolean();
            agRights.Control = row["Control"].ObjectToBoolean();
            agRights.Owner = row["Owner"].ObjectToBoolean();
            agRights.ListView = row["ListView"].ObjectToBoolean();
            agRights.Save = row["Save"].ObjectToBoolean();
            agRights.SecurityAdmin = row["SecurityAdmin"].ObjectToBoolean();
            agRights.Hide = row["Hide"].ObjectToBoolean();
            agRights.Unhide = row["Unhide"].ObjectToBoolean();
            agRights.AuthorizeAll = row["AuthorizeAll"].ObjectToBoolean();
            agRights.AuthorizeReading = row["AuthorizeReading"].ObjectToBoolean();
            agRights.AuthorizeShare = row["AuthorizeShare"].ObjectToBoolean();
            agRights.AuthorizeControl = row["AuthorizeControl"].ObjectToBoolean();
            agRights.AuthorizeExecution = row["AuthorizeExecution"].ObjectToBoolean();
            agRights.AuthorizeAdd = row["AuthorizeAdd"].ObjectToBoolean();
            agRights.AuthorizeEdit = row["AuthorizeEdit"].ObjectToBoolean();
            agRights.Extra = row["Extra"].ObjectToBoolean();
            agRights.AuthorizationLimit = row["AuthorizationLimit"].ObjectToDateTime();
            agRights.ExtraRights = row["ExtraRights"].ObjectToInt();
            agRights.FkRightsModel = row["FkRightsModel"].ObjectToLong();
            agRights._RightsTime_ = row["_RightsTime_"].ObjectToBoolean();
            agRights.StartAt = row["StartAt"].ObjectToDateTime();
            agRights.EndAt = row["EndAt"].ObjectToDateTime();
            agRights.AtHolydays = row["AtHolydays"].ObjectToBoolean();
            agRights.UseMaps = row["UseMaps"].ObjectToBoolean();
            agRights.AtSundayMap = row["AtSundayMap"].ObjectToString();
            agRights.AtMondayMap = row["AtMondayMap"].ObjectToString();
            agRights.AtTuesdayMap = row["AtTuesdayMap"].ObjectToString();
            agRights.AtWednesdayMap = row["AtWednesdayMap"].ObjectToString();
            agRights.AtThursdayMap = row["AtThursdayMap"].ObjectToString();
            agRights.AtFridayMap = row["AtFridayMap"].ObjectToString();
            agRights.AtSaturdayMap = row["AtSaturdayMap"].ObjectToString();
            agRights.AtOddMap = row["AtOddMap"].ObjectToString();
            agRights.AtEvenMap = row["AtEvenMap"].ObjectToString();
            agRights._Authentication_ = row["_Authentication_"].ObjectToBoolean();
            agRights.NeedAuth = row["NeedAuth"].ObjectToBoolean();
            agRights.Signature = row["Signature"].ObjectToString();
            agRights.FkSigner =row["FkSigner"].ObjectToLong();
            agRights.Hash = row["Hash"].ObjectToString();
            agRights._Control_ = row["_Control_"].ObjectToBoolean();
            agRights.FkCreationUser = row["FkCreationUser"].ObjectToLong();
            agRights.CreationDate = row["CreationDate"].ObjectToDateTime();
            agRights.FkLastUpdateUser =row["FkLastUpdateUser"].ObjectToLong();
            agRights.UpdateDate = row["UpdateDate"].ObjectToDateTime();
            agRights.FkMainId = row["FkMainId"].ObjectToLong();
            agRights.FkDataStatus =row["FkDataStatus"].ObjectToInt();

            return agRights;
        }

        private AgContentView MakeContent(DataRow row)
        {

            AgContentView agContent = new AgContentView();
             
            agContent.ID = row["ID"].ObjectToLong();
            agContent.FkDataType = row["FkDataType"].ObjectToLong();
            agContent.Description = row["Description"].ObjectToString() ;
            agContent.Alias = row["Alias"].ObjectToString();
            agContent._MainLink_ = row["_MainLink_"].ObjectToBoolean();
            agContent.FkLinkType = row["FkLinkType"].ObjectToLong();
            agContent.FkSource = row["FkSource"].ObjectToLong();
            agContent.FkTarget = row["FkTarget"].ObjectToLong();
            agContent.Sequence = row["Sequence"].ObjectToInt();
            agContent._MainContentInfo_ = row["_MainContentInfo_"].ObjectToBoolean();
            agContent.KEY = row["KEY"].ObjectToString();
            agContent.FkLanguage = row["FkLanguage"].ObjectToLong();
            agContent.FkRoot = row["FkRoot"].ObjectToLong();
            agContent.Paper = row["Paper"].ObjectToString();
            agContent.FkPaperType = row["FkPaperType"].ObjectToLong();
            agContent.FkThumb = row["FkThumb"].ObjectToLong();
            agContent.StructuredCode = row["StructuredCode"].ObjectToString();
            agContent.FkStatus = row["FkStatus"].ObjectToLong();
            agContent._Dates_ = row["_Dates_"].ObjectToBoolean();
            agContent.Start = row["Start"].ObjectToDateTime();
            agContent.End = row["End"].ObjectToDateTime();
            agContent.ScheduleStart = row["ScheduleStart"].ObjectToDateTime();
            agContent.ScheduleEnd = row["ScheduleEnd"].ObjectToDateTime();
            agContent._Content_ = row["_Content_"].ObjectToBoolean();
            agContent.FkFile = row["FkFile"].ObjectToLong();
            agContent.FkUrl = row["FkUrl"].ObjectToLong();
            agContent.TextualContent = row["TextualContent"].ObjectToString();
            agContent.Meta = row["Meta"].ObjectToString();
            agContent._Values_ = row["_Values_"].ObjectToBoolean();
            agContent.Evaluation = row["Evaluation"].ObjectToInt();
            agContent.OutValue = row["OutValue"].ObjectToDecimal();
            agContent.InValue = row["InValue"].ObjectToDecimal();
            agContent.FkCurrency = row["FkCurrency"].ObjectToLong();
            agContent.Factor = row["Factor"].ObjectToDecimal();
            agContent.ThisQtt = row["ThisQtt"].ObjectToDecimal();
            agContent.ThisValue = row["ThisValue"].ObjectToDecimal();
            agContent.ThisCost = row["ThisCost"].ObjectToDecimal();
            agContent.ThisProfit = row["ThisProfit"].ObjectToDecimal();
            agContent.ThisPrice = row["ThisPrice"].ObjectToDecimal();
            agContent._GeoReference_ = row["_GeoReference_"].ObjectToBoolean();
            agContent.IsAddressSource = row["IsAddressSource"].ObjectToBoolean();
            agContent.Latitude = row["Latitude"].ObjectToString();
            agContent.Longitude = row["Longitude"].ObjectToString();
            agContent.Altitude = row["Altitude"].ObjectToString();
            agContent._Authentication_ = row["_Authentication_"].ObjectToBoolean();
            agContent.NeedSignature = row["NeedSignature"].ObjectToBoolean();
            agContent.FkSigner = row["FkSigner"].ObjectToLong();
            agContent._Control_ = row["_Control_"].ObjectToBoolean();
            agContent.IsProfile = row["IsProfile"].ObjectToBoolean();
            agContent.Encrypted = row["Encrypted"].ObjectToBoolean();
            agContent.Public = row["Public"].ObjectToBoolean();
            agContent.System = row["System"].ObjectToBoolean();
            agContent.SelfContent = row["SelfContent"].ObjectToBoolean();
            agContent.FkCreationUser = row["FkCreationUser"].ObjectToLong();
            agContent.CreationDate = row["CreationDate"].ObjectToDateTime();
            agContent.FkLastUpdateUser = row["FkLastUpdateUser"].ObjectToLong();
            agContent.UpdateDate = row["UpdateDate"].ObjectToDateTime();
            agContent.FkMainId = row["FkMainId"].ObjectToLong();
            agContent.FkDataStatus = row["FkDataStatus"].ObjectToInt();
            return agContent;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //if (!this.AgContents.Any())
            //{
                //builder.Entity<AgContent>().HasData(
                //   //GetContents()
                //);
            //}


            //if (!this.AgRights.Any())
            //{
                //builder.Entity<AgRight>().HasData(
                //    //GetRights()
                //);
            //}

        }

        //public DbSet<AgContent> AgContents { get; set; }
        public DbSet<AgContent> AgContent { get; set; }
        public DbSet<AgRight> AgRight { get; set; }
        public DbSet<AgAuth> AgAuth { get; set; }

    }

}
