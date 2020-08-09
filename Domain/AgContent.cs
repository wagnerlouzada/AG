using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ag.Domain
{

    public class AgContentView: AgContent
    {
        public String DataTypeDescription { get; set; }
        public String LinkTypeDescription { get; set; }
        public String SourceDescription { get; set; }
        public String TargetDescription { get; set; }
        public String LanguageDescription { get; set; }
        public String RootDescription { get; set; }
        public String PaperTypeDescription { get; set; }
        public String ThumbDescription { get; set; }
        public String StatusDescription { get; set; }
        public String FileDescription { get; set; }
        public String UrlDescription { get; set; }
        public String CurrencyDescription { get; set; }
        public String CreationUserDescription { get; set; }
        public String LastUpdateUserDescription { get; set; }
        public String MainIdDescription { get; set; }
        public String DataStatusDescription { get; set; }
        public String SignerDescription { get; set; }
    }

    public class AgContent
    {
        public long ID { get; set; }
        public long FkDataType { get; set; }
        public String Description { get; set; } // nvarchar(450) 
        public String Alias { get; set; } // nvarchar(450) 
        public Boolean? _MainLink_ { get; set; }
        public long? FkLinkType { get; set; }
        public long? FkSource { get; set; }
        public long? FkTarget { get; set; }
        public int? Sequence { get; set; }
        public Boolean? _MainContentInfo_ { get; set; }
        public String KEY { get; set; } //varchar(50) {
        public long? FkLanguage { get; set; }
        public long? FkRoot { get; set; }
        public String Paper { get; set; } // char(50)
        public long? FkPaperType { get; set; }
        public long? FkThumb { get; set; }
        public String StructuredCode { get; set; } // varchar(50) 
        public long? FkStatus { get; set; }
        public Boolean? _Dates_ { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? ScheduleStart { get; set; }
        public DateTime? ScheduleEnd { get; set; }
        public Boolean? _Content_ { get; set; }
        public long? FkFile { get; set; }
        public long? FkUrl { get; set; }
        public String TextualContent { get; set; }
        public String Meta { get; set; }
        public Boolean? _Values_ { get; set; }
        public int? Evaluation { get; set; }
        public decimal? OutValue { get; set; } // (18, 4)
        public decimal? InValue { get; set; }// (18, 4)
        public long? FkCurrency { get; set; }
        public decimal? Factor { get; set; }// (18, 4)
        public decimal? ThisQtt { get; set; } // (18, 4)
        public decimal? ThisValue { get; set; } // (18, 4)
        public decimal? ThisCost { get; set; }// (18, 4)
        public decimal? ThisProfit { get; set; }// (18, 4)
        public decimal? ThisPrice { get; set; }// (18, 4)
        public Boolean? _GeoReference_ { get; set; }
        public Boolean? IsAddressSource { get; set; }
        public String Latitude { get; set; } // (40)
        public String Longitude { get; set; } // (40)
        public String Altitude { get; set; } // (40)
        public Boolean? _Authentication_ { get; set; }
        public Boolean? NeedSignature { get; set; }
        public long? FkSigner { get; set; }
        public Boolean? _Control_ { get; set; }
        public Boolean? IsProfile { get; set; }
        public Boolean? Encrypted { get; set; }
        public Boolean? Public { get; set; }
        public Boolean? System { get; set; }
        public Boolean? SelfContent { get; set; }
        public long? FkCreationUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? FkLastUpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? FkMainId { get; set; }
        public long? FkDataStatus { get; set; }

    }

    public class AgRightView: AgRight
    {
        public String ContentDescription { get; set; }
        public String UserDescription { get; set; }
        public String NotificationStatusDescription { get; set; }
        public String RightModelDescription { get; set; }
        public String DataTypeDescription { get; set; }
        public String SignerDescription { get; set; }
        public String CreationUserDescription { get; set; }
        public String LastUpdateUserDescription { get; set; }
        public String MainIdDescription { get; set; }
        public String DataStatusDescription { get; set; }
    }

    public class AgRight
    {
        public long ID { get; set; }
        public long FkContent { get; set; }
        public long FkUser { get; set; }
        public long? FkNotificationStatus { get; set; }
        public Boolean? _DestinationRights_ { get; set; }
        public Boolean? Create { get; set; }
        public Boolean? Read { get; set; }
        public Boolean? Update { get; set; }
        public Boolean? Delete { get; set; }
        public Boolean? Write { get; set; }
        public Boolean? Execute { get; set; }
        public Boolean? DelegateRights { get; set; }
        public Boolean? Share { get; set; }
        public Boolean? Control { get; set; }
        public Boolean? Owner { get; set; }
        public Boolean? ListView { get; set; }
        public Boolean? Save { get; set; }
        public Boolean? SecurityAdmin { get; set; }
        public Boolean? Hide { get; set; }
        public Boolean? Unhide { get; set; }
        public Boolean? AuthorizeAll { get; set; }
        public Boolean? AuthorizeReading { get; set; }
        public Boolean? AuthorizeShare { get; set; }
        public Boolean? AuthorizeControl { get; set; }
        public Boolean? AuthorizeExecution { get; set; }
        public Boolean? AuthorizeAdd { get; set; }
        public Boolean? AuthorizeEdit { get; set; }
        public Boolean? Extra { get; set; }
        public DateTime? AuthorizationLimit { get; set; }
        public int? ExtraRights { get; set; }
        public long? FkRightsModel { get; set; }
        public Boolean? _RightsTime_ { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public Boolean? AtHolydays { get; set; }
        public Boolean? UseMaps { get; set; }
        public String AtSundayMap { get; set; } // (12)
        public String AtMondayMap { get; set; } // (12)
        public String AtTuesdayMap { get; set; } // (12)
        public String AtWednesdayMap { get; set; } // (12)
        public String AtThursdayMap { get; set; } // (12)
        public String AtFridayMap { get; set; } // (12)
        public String AtSaturdayMap { get; set; } // (12)
        public String AtOddMap { get; set; } // (12)
        public String AtEvenMap { get; set; } // (12)
        public Boolean? _Authentication_ { get; set; }
        public Boolean? NeedAuth { get; set; }
        public String Signature { get; set; } //(256)
        public long? FkSigner { get; set; }
        public String Hash { get; set; } //        (50)
        public Boolean? _Control_ { get; set;  }
        public long? FkCreationUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? FkLastUpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? FkMainId { get; set; }
        public long? FkDataStatus { get; set; }

    }

    public class AgAuthView: AgAuth // dados capturados via relacionamento, não são armazenados
    {
        public String RightDescription { get; set; }
        public String SignatureDescription { get; set; }
        public String SignerDescription { get; set; }
        public String CreationUserDescription { get; set; }
        public String LastUpdateUserDescription { get; set; }
        public String MainIdDescription { get; set; }
        public String DataStatusDescription { get; set; }
    }

    public class AgAuth
    {
        public long ID { get; set; }
        public long FkRight { get; set; }
        public String ContentKey { get; set; } //        (50)
        public long FkSigner { get; set; }
        public String FkSignature { get; set; } //        (256)
        public String Hash { get; set; } //        (50)
        public Boolean _Control_ { get; set; }
        public long FkCreationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public long FkLastUpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public long FkMainId { get; set; }
        public long FkDataStatus { get; set; }

    }

}
