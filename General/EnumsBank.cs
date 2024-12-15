using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.General
{
    #region Person
    public enum GenderMode
    {
        Null = -1,
        Male,
        Female
    }
    public enum MaritalMode
    {
        Null = -1,
        Single,
        Married
    }
    public enum AccessMode
    {
        Null = -1,
        Guest,
        User,
        Operator,
        Administrator
    }
    #endregion

    #region Date & Time
    public enum GeneralAgeMode
    {
        Modern,
        Classical,
        Historical
    }
    public enum DayOfWeekMode
    {
        Null = -1,
        Saturday = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6
    }
    public enum MonthMode
    {
        Null = -1,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public enum PeriodMode
    {
        Null = -1,
        Day,
        Week,
        Month,
        Year,
        Decade,
        Century,
        Millennium
    }
    public enum VacationMode
    {
        Null = -1,
        Official,
        NonOfficial
    }
    public enum DayMode
    {
        Null = -1,
        WorkDay,
        OfficialHoliday,
        NonOfficialHoliday,
        RestDay,
        LeaveDay,
        TestDay
    }
    public enum TimeZoneMode
    {
        Null = -1
        , Acre
        , Afghanistan
        , AIXSpecificEquivalentOfCentralEuropean
        , AlaskaDaylight
        , AlaskaStandard
        , AmazonSummer_Brazil
        , AmazonBrazil
        , ArabiaStandard
        , Argentina
        , Armenia
        , ASEANCommon
        , AtlanticDaylight
        , AtlanticStandard
        , AustralianCentralDaylightSavings
        , AustralianCentralStandard
        , AustralianEasternDaylightSavings
        , AustralianEasternStandard
        , AustralianWesternDaylight
        , AustralianWesternStandard
        , Azerbaijan
        , AzoresStandard
        , BakerIsland
        , BangladeshDaylight_BangladeshDaylightSaving
        , BangladeshStandard
        , Bhutan
        , Bolivia
        , BougainvilleStandard
        , BrasiliaSummer
        , Brasilia
        , BritishIndianOcean
        , BritishSummer_BritishStandard
        , Brunei
        , CapeVerde
        , CentralAfrica
        , CentralDaylight_NorthAmerica
        , CentralEuropeanDaylight
        , CentralEuropeanSummer_CfHAEC
        , CentralEuropean
        , CentralIndonesia
        , CentralStandard_Australia
        , CentralStandard_NorthAmerica
        , CentralSummer_Australia
        , CentralWesternStandard_AustraliaUnofficial
        , ChamorroStandard
        , ChathamDaylight
        , ChathamStandard
        , ChileStandard
        , ChileSummer
        , ChinaStandard
        , China
        , Choibalsan
        , ChristmasIsland
        , Chuuk
        , ClippertonIslandStandard
        , CocosIslands
        , ColombiaSummer
        , Colombia
        , CookIsland
        , CoordinatedUniversal
        , CubaDaylight
        , CubaStandard
        , Davis
        , DumontDUrville
        , EastAfrica
        , EasterIslandStandardSummer
        , EasterIslandStandard
        , EasternCaribbean_DoesNotRecogniseDST
        , EasternDaylight_NorthAmerica
        , EasternEuropeanDaylight
        , EasternEuropeanSummer
        , EasternEuropean
        , EasternGreenlandSummer
        , EasternGreenland
        , EasternIndonesian
        , EasternStandard_Australia
        , EasternStandard_NorthAmerica
        , Ecuador
        , FalklandIslandsStandard
        , FalklandIslandsSummer
        , FalklandIslands
        , FernandoDeNoronha
        , Fiji
        , FrenchGuiana
        , FurtherEasternEuropean
        , Galapagos
        , GambierIsland
        , GambierIslands
        , GeorgiaStandard
        , GilbertIsland
        , GreenwichMean
        , GulfStandard
        , Guyana
        , HawaiiStandard
        , HawaiiAleutianDaylight
        , HawaiiAleutianStandard
        , HeardAndMcDonaldIslands
        , HeureAvancéeD_EuropeCentraleFrancisedNameForCEST
        , HongKong
        , IndianOcean
        , IndianStandard
        , IndianKerguelen
        , Indochina
        , InternationalBusinessStandard
        , IranDaylight
        , IranStandard
        , IrishStandard
        , Irkutsk
        , IsraelDaylight
        , IsraelStandard
        , JapanStandard
        , Kaliningrad
        , Kamchatka
        , Khovd
        , KoreaStandard
        , Kosrae
        , Krasnoyarsk
        , Kyrgyzstan
        , LineIslands
        , LordHoweStandard
        , LordHoweSummer
        , MacquarieIslandStation
        , Magadan
        , MalaysiaStandard
        , Malaysia
        , Maldives
        , MarquesasIslands
        , MarshallIslands
        , Mauritius
        , MawsonStation
        , MiddleEuropeanSummer_SameZoneAsCEST
        , MiddleEuropean_SameZoneAsCET
        , Moscow
        , MountainDaylight_NorthAmerica
        , MountainStandard_NorthAmerica
        , MyanmarStandard
        , Myanmar
        , Nepal
        , NewCaledonia
        , NewZealandDaylight
        , NewZealandStandard
        , NewfoundlandDaylight
        , NewfoundlandStandard
        , Newfoundland
        , Niue
        , Norfolk
        , Omsk
        , Oral
        , PacificDaylight_NorthAmerica
        , PacificStandard_NorthAmerica
        , PakistanStandard
        , PapuaNewGuinea
        , ParaguaySummer_SouthAmerica
        , Paraguay_SouthAmerica
        , Peru
        , PhilippineStandard
        , PhoenixIsland
        , PohnpeiStandard
        , RotheraResearchStation
        , Réunion
        , SaintPierreAndMiquelonDaylight
        , SaintPierreAndMiquelonStandard
        , SakhalinIsland
        , Samara
        , SamoaStandard
        , Seychelles
        , ShowaStation
        , SingaporeStandard
        , Singapore
        , SolomonIslands
        , SouthAfricanStandard
        , SouthGeorgiaAndTheSouthSandwichIslands
        , Srednekolymsk
        , SriLankaStandard
        , Suriname
        , Tahiti
        , Tajikistan
        , ThailandStandard
        , TimorLeste
        , Tokelau
        , Tonga
        , Turkmenistan
        , Tuvalu
        , Ulaanbaatar
        , UruguayStandard
        , UruguaySummer
        , Uzbekistan
        , Vanuatu
        , VenezuelanStandard
        , Vladivostok
        , Volgograd
        , VostokStation
        , WakeIsland
        , WestAfricaSummer
        , WestAfrica
        , WesternEuropeanDaylight
        , WesternEuropeanSummer
        , WesternEuropean
        , WesternIndonesian
        , WesternStandard
        , Yakutsk
        , Yekaterinburg
        , Zulu
    }
    #endregion

    #region Work & Education
    public enum ExamMode
    {
        Null = -1,
        Intelligent = 0,
        Network = 1,
        Sheet = 2,
        Website = 3
    }
    public enum EducationalDegreeMode
    {
        Null = -1,
        UnLiterate,
        LowLiteracy,
        Literacy,
        VocationalDiploma,
        Diploma,
        AssociateDegree,
        BSc,
        MSc,
        PhD,
        PostDoctoral
    }
    public enum ScoreStateMode
    {
        Null = -1,
        Positive,
        Negative,
        Both
    }
    public enum WorkLevelMode
    {
        Null = -1,
        HourlyWorker,
        ContractWorker,
        SimpleWorker,
        Foreman,
        HourlyEmployee,
        ContractEmployee,
        SimpleEmployee,
        SpecializedEmployee,
        DeputySection,
        HeadedSection,
        DeputyAreas,
        HeadedAreas,
        DeputyGeneral,
        Chief
    }
    public enum ShiftMode
    {
        Null = -1,
        Fixed,
        Rotation
    }
    public enum LeaveMode
    {
        Null = -1,
        Paid,
        Sick,
        UnPaid,
        Case,
        Parturition,
        Marriage
    }
    public enum PersonInsuranceMode
    {
        Null = -1,
        UnInsured,
        HealthInsurance,
        PensionInsurance,
        PensionAndHealthInsurance
    }

    #endregion

    #region Math
    public enum LayoutMode
    {
        Null = -1,
        Both,
        Horizental,
        Vertical
    }
    public enum WeightCompareMode
    {
        Null = -1,
        Equals,
        Low,
        More
    }
    public enum GrowSideMode
    {
        Null = -1,
        Decreasing,
        Increasing,
        Equals
    }
    public enum ImportanceMode
    {
        Null = -1,
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh,
    }
    public enum ConfigMode
    {
        Null = -1,
        Default,
        Equal,
        Special
    }
    public enum OrderMode
    {
        Null = -1,
        OrderBy,
        Random,
        Special
    }
    public enum StateMode
    {
        Negative = -1,
        Middle = 0,
        Positive = 1
    }
    #endregion

    #region Dialog
    public enum MessageMode
    {
        Null = -1,
        Message,
        Warning,
        Question,
        Success,
        Error
    }
    #endregion

    #region Localization

    public enum LanguageMode
    {
        Null = -1,
        Abkhazian ,
        Acehnese ,
        Afar ,
        Afrikaans ,
        Akan ,
        Albanian ,
        Alemannic ,
        Amharic ,
        AngloSaxon ,
        Arabic ,
        Aragonese ,
        Aramaic ,
        Armenian ,
        Aromanian ,
        Assamese ,
        Asturian ,
        Avar ,
        Aymara ,
        Azerbaijani ,
        Bambara ,
        Banjar ,
        Banyumasan ,
        Bashkir ,
        Basque ,
        Bavarian ,
        Belarusian ,
        BelarusianTaraškievica ,
        Bengali ,
        Bihari ,
        BishnupriyaManipuri ,
        Bislama ,
        Bosnian ,
        Breton ,
        Buginese ,
        Bulgarian ,
        Burmese ,
        Buryat ,
        Cantonese ,
        Catalan ,
        Cebuano ,
        CentralBicolano ,
        Chamorro ,
        Chechen ,
        Cherokee ,
        Cheyenne ,
        Chichewa ,
        Chinese ,
        Choctaw ,
        Chuvash ,
        ClassicalChinese ,
        Cornish ,
        Corsican ,
        Cree ,
        CrimeanTatar ,
        Croatian ,
        Czech ,
        Danish ,
        Divehi ,
        Dutch ,
        DutchLowSaxon ,
        Dzongkha ,
        EgyptianArabic ,
        EmilianRomagnol ,
        English ,
        Erzya ,
        Esperanto ,
        Estonian ,
        Ewe ,
        Extremaduran ,
        Faroese ,
        Fijian ,
        FijiHindi ,
        Finnish ,
        FrancoProvençal ,
        French ,
        Friulian ,
        Fula ,
        Gagauz ,
        Galician ,
        Gan ,
        Georgian ,
        German ,
        Gilaki ,
        GoanKonkani ,
        Gothic ,
        Greek ,
        Greenlandic ,
        Guarani ,
        Gujarati ,
        Haitian ,
        Hakka ,
        Hausa ,
        Hawaiian ,
        Hebrew ,
        Herero ,
        HillMari ,
        Hindi ,
        HiriMotu ,
        Hungarian ,
        Icelandic ,
        Ido ,
        Igbo ,
        Ilokano ,
        Indonesian ,
        Interlingua ,
        Interlingue ,
        Inuktitut ,
        Inupiak ,
        Irish ,
        Italian ,
        Japanese ,
        Javanese ,
        KabardianCircassian ,
        Kabyle ,
        Kalmyk ,
        Kannada ,
        Kanuri ,
        Kapampangan ,
        KarachayBalkar ,
        Karakalpak ,
        Kashmiri ,
        Kashubian ,
        Kazakh ,
        Khmer ,
        Kikuyu ,
        Kinyarwanda ,
        Kirghiz ,
        Kirundi ,
        Komi ,
        KomiPermyak ,
        Kongo ,
        Korean ,
        Kuanyama ,
        Kurdish ,
        Ladino ,
        Lak ,
        Lao ,
        Latgalian ,
        Latin ,
        Latvian ,
        Lezgian ,
        Ligurian ,
        Limburgish ,
        Lingala ,
        Lithuanian ,
        Lojban ,
        Lombard ,
        LowerSorbian ,
        LowSaxon ,
        Luganda ,
        Luxembourgish ,
        Macedonian ,
        Maithili ,
        Malagasy ,
        Malay ,
        Malayalam ,
        Maltese ,
        Manx ,
        Maori ,
        Marathi ,
        Marshallese ,
        Mazandarani ,
        MeadowMari ,
        Minangkabau ,
        MinDong ,
        Mingrelian ,
        MinNan ,
        Mirandese ,
        Moksha ,
        Moldovan ,
        Mongolian ,
        Muscogee ,
        Nahuatl ,
        Nauruan ,
        Navajo ,
        Ndonga ,
        Neapolitan ,
        Nepali ,
        Newar ,
        Norfolk ,
        Norman ,
        NorthernLuri ,
        NorthernSami ,
        NorthernSotho ,
        NorthFrisian ,
        NorwegianBokmål ,
        NorwegianNynorsk ,
        Novial ,
        Occitan ,
        OldChurchSlavonic ,
        Oriya ,
        Oromo ,
        Ossetian ,
        PalatinateGerman ,
        Pali ,
        Pangasinan ,
        Papiamentu ,
        Pashto ,
        PennsylvaniaGerman ,
        Persian ,
        Picard ,
        Piedmontese ,
        Polish ,
        Pontic ,
        Portuguese ,
        Punjabi ,
        Quechua ,
        Ripuarian ,
        Romani ,
        Romanian ,
        Romansh ,
        Russian ,
        Rusyn ,
        Sakha ,
        Samoan ,
        Samogitian ,
        Sango ,
        Sanskrit ,
        Sardinian ,
        SaterlandFrisian ,
        Scots ,
        ScottishGaelic ,
        Serbian ,
        SerboCroatian ,
        Sesotho ,
        Shona ,
        SichuanYi ,
        Sicilian ,
        Silesian ,
        SimpleEnglish ,
        Sindhi ,
        Sinhalese ,
        Slovak ,
        Slovenian ,
        Somali ,
        Sorani ,
        SouthAzerbaijani ,
        Spanish ,
        Sranan ,
        Sundanese ,
        Swahili ,
        Swati ,
        Swedish ,
        Tagalog ,
        Tahitian ,
        Tajik ,
        Tamil ,
        Tarantino ,
        Tatar ,
        Telugu ,
        Tetum ,
        Thai ,
        Tibetan ,
        Tigrinya ,
        TokPisin ,
        Tongan ,
        Tsonga ,
        Tswana ,
        Tumbuka ,
        Turkish ,
        Turkmen ,
        Tuvan ,
        Twi ,
        Udmurt ,
        Ukrainian ,
        UpperSorbian ,
        Urdu ,
        Uyghur ,
        Uzbek ,
        Venda ,
        Venetian ,
        Vepsian ,
        Vietnamese ,
        Volapük ,
        Võro ,
        Walloon ,
        WarayWaray ,
        Welsh ,
        WesternPunjabi ,
        WestFlemish ,
        WestFrisian ,
        Wolof ,
        Wu ,
        Xhosa ,
        Yiddish ,
        Yoruba ,
        ZamboangaChavacano ,
        Zazaki ,
        Zeelandic ,
        Zhuang ,
        Zulu
    }
    public enum CountryMode
    {
        Null=-1
        ,Afghanistan  
        ,Albania  
        ,Algeria  
        ,AmericanSamoa  
        ,Andorra  
        ,Angola  
        ,Anguilla  
        ,AntiguaAndBarbuda  
        ,Argentina  
        ,Armenia  
        ,Aruba  
        ,Australia  
        ,Austria  
        ,Azerbaijan  
        ,Bahamas  
        ,Bahrain  
        ,Bangladesh  
        ,Barbados  
        ,Belarus  
        ,Belgium  
        ,Belize  
        ,Benin  
        ,Bermuda  
        ,Bhutan  
        ,Bolivia  
        ,Bonaire  
        ,BosniaHerzegovina  
        ,Botswana  
        ,BouvetIsland  
        ,Brazil  
        ,Brunei  
        ,Bulgaria  
        ,BurkinaFaso  
        ,Burundi  
        ,Cambodia  
        ,Cameroon  
        ,Canada  
        ,CapeVerde  
        ,CaymanIslands  
        ,CentralAfricanRepublic  
        ,Chad  
        ,Chile  
        ,China  
        ,ChristmasIsland  
        ,CocosKeelingIslands  
        ,Colombia  
        ,Comoros  
        ,Congo_DemocraticRepublicOfTheZaire  
        ,Congo_RepublicOf  
        ,CookIslands  
        ,CostaRica  
        ,Croatia  
        ,Cuba  
        ,Curacao  
        ,Cyprus  
        ,CzechRepublic  
        ,Denmark  
        ,Djibouti  
        ,Dominica  
        ,DominicanRepublic  
        ,Ecuador  
        ,Egypt  
        ,ElSalvador  
        ,EquatorialGuinea  
        ,Eritrea  
        ,Estonia  
        ,Ethiopia  
        ,FalklandIslands  
        ,FaroeIslands  
        ,Fiji  
        ,Finland  
        ,France  
        ,FrenchGuiana  
        ,Gabon  
        ,Gambia  
        ,Georgia  
        ,Germany  
        ,Ghana  
        ,Gibraltar  
        ,Greece  
        ,Greenland  
        ,Grenada  
        ,GuadeloupeFrench  
        ,GuamUSA  
        ,Guatemala  
        ,Guinea  
        ,GuineaBissau  
        ,Guyana  
        ,Haiti  
        ,HolySee  
        ,Honduras  
        ,HongKong  
        ,Hungary  
        ,Iceland  
        ,India  
        ,Indonesia  
        ,Iran  
        ,Iraq  
        ,Ireland  
        ,Israel  
        ,Italy  
        ,IvoryCoastCoteDIvoire  
        ,Jamaica  
        ,Japan  
        ,Jordan  
        ,Kazakhstan  
        ,Kenya  
        ,Kiribati  
        ,Kosovo  
        ,Kuwait  
        ,Kyrgyzstan  
        ,Laos  
        ,Latvia  
        ,Lebanon  
        ,Lesotho  
        ,Liberia  
        ,Libya  
        ,Liechtenstein  
        ,Lithuania  
        ,Luxembourg  
        ,Macau  
        ,Macedonia  
        ,Madagascar  
        ,Malawi  
        ,Malaysia  
        ,Maldives  
        ,Mali  
        ,Malta  
        ,MarshallIslands  
        ,MartiniqueFrench  
        ,Mauritania  
        ,Mauritius  
        ,Mayotte  
        ,Mexico  
        ,Micronesia  
        ,Moldova  
        ,Monaco  
        ,Mongolia  
        ,Montenegro  
        ,Montserrat  
        ,Morocco  
        ,Mozambique  
        ,Myanmar  
        ,Namibia  
        ,Nauru  
        ,Nepal  
        ,Netherlands  
        ,NetherlandsAntilles  
        ,NewCaledoniaFrench  
        ,NewZealand  
        ,Nicaragua  
        ,Niger  
        ,Nigeria  
        ,Niue  
        ,NorfolkIsland  
        ,NorthKorea  
        ,NorthernMarianaIslands  
        ,Norway  
        ,Oman  
        ,Pakistan  
        ,Palau  
        ,Panama  
        ,PapuaNewGuinea  
        ,Paraguay  
        ,Peru  
        ,Philippines  
        ,PitcairnIsland  
        ,Poland  
        ,PolynesiaFrench  
        ,Portugal  
        ,PuertoRico  
        ,Qatar  
        ,Reunion  
        ,Romania  
        ,Russia  
        ,Rwanda  
        ,SaintHelena  
        ,SaintKittsAndNevis  
        ,SaintLucia  
        ,SaintPierreAndMiquelon  
        ,SaintVincentAndGrenadines  
        ,Samoa  
        ,SanMarino  
        ,SaoTomeAndPrincipe  
        ,SaudiArabia  
        ,Senegal  
        ,Serbia  
        ,Seychelles  
        ,SierraLeone  
        ,Singapore  
        ,SintMaarten  
        ,Slovakia  
        ,Slovenia  
        ,SolomonIslands  
        ,Somalia  
        ,SouthAfrica  
        ,SouthGeorgiaAndSouthSandwichIslands  
        ,SouthKorea  
        ,SouthSudan  
        ,Spain  
        ,SriLanka  
        ,Sudan  
        ,Suriname  
        ,SvalbardAndJanMayenIslands  
        ,Swaziland  
        ,Sweden  
        ,Switzerland  
        ,Syria  
        ,Taiwan  
        ,Tajikistan  
        ,Tanzania  
        ,Thailand  
        ,TimorLesteEastTimor  
        ,Togo  
        ,Tokelau  
        ,Tonga  
        ,TrinidadAndTobago  
        ,Tunisia  
        ,Turkey  
        ,Turkmenistan  
        ,TurksAndCaicosIslands  
        ,Tuvalu  
        ,Uganda  
        ,Ukraine  
        ,UnitedArabEmirates  
        ,UnitedKingdom  
        ,UnitedStates  
        ,Uruguay  
        ,Uzbekistan  
        ,Vanuatu  
        ,Venezuela  
        ,Vietnam  
        ,VirginIslands  
        ,WallisAndFutunaIslands  
        ,Yemen  
        ,Zambia  
        ,Zimbabwe  
        ,xk  
    }
   
    #endregion

    #region Position
    public enum SidePositionMode
    {
        Null = -1,
        Left,
        Top,
        Right,
        Bottom
    }
    public enum PositionMode
    {
        Null = -1,
        Left,
        Top,
        Center,
        Right,
        Bottom
    }
    public enum RouteMode
    {
        Null = -1,
        Start,
        Middle,
        End
    }

    #endregion

    #region Extension
    public enum ImageExtensionMode
    {
        Null= -1,
        AllImageFile,
        jpg,
        jpeg,
        png, 
        gif,
        bmp,
        tif
    }
    public enum ExecuteMode
    {
        Null = -1,
        ExecuteScalar,
        ExecuteNonQuery,
        ExecuteReader,
        ExecuteScalarAsync,
        ExecuteNonQueryAsync,
        ExecuteReaderAsync
    }
    public enum QuestionMode
    {
        Null = -1,
        Descriptive,
        Supplementary,
        MultipleChoice
    }
    #endregion

    #region Programming
    public enum ProgrammingAccessMode
    {
        Public = 0, Internal = 1, Protected = 2, Private = 3
    }
    public enum ThreadingMethodMode
    {
        Null = -1,Default = 0, SingleThread = 1, MultiThread = 2, SingleTask = 3, MultiTask = 4, BackgroundWorker = 5
    }
    public enum BooleanMode
    {
        Null = -1,
        False = 0,
        True = 1
    }
    public enum SimilarityMode
    {
        Null = -1,
        This = 0,
        Equal = 1,
        Same = 2,
        Like = 3,
        Congruent = 4,
        Pattern = 5
    }
    public enum UsageMode
    {
        Null = -1,
        Get = 0,
        Set = 1
    }
    public enum LinkMode
    {
        Null = -1,
        Pointer = 0,
        InternalPage = 1,
        ExternalPage = 2,
        Download = 3
    }
    public enum TableValuePositionMode
    {
        Null = -1,
        NextRowCell = 0,
        NextColumnCell = 1,
        NextSubCell = 2
    }
    public enum TableChangeMode
    {
        Null = -1,
        Insert = 0,
        Modify = 1,
        Delete = 2
    }

    public enum StandardTypes
    {
        C = 0,
        Java = 1,
        MSSQL = 2,
        MySQL = 3,
        SQLite = 4,
        PostgreSQL = 5
    }
    #endregion
}
