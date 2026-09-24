using DynamicATS.Common;
using DynamicATS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinDTO;
using System.Windows.Input;
using WinDTO.Tasks;
using WinUIDTO.Interfaces;
using System.ComponentModel;
using WinUIDTO;
using System.Threading;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;
using iTextSharp.text;
//using WebSupergoo.ABCpdf9;
using iTextSharp.text.pdf;
using System.IO;
using System.Windows;
using Utilities;
using System.Data;

//using word = Microsoft.Office.Interop.Word;


namespace DynamicATS.ViewModel
{
    public class AuditApprovalTaskVM : TaskBaseVM<AuditApprovedPrescriptionDetails>, IControlAccessHandler, IWindowTimeHandler
    {
        #region constants

        public AuditApprovalTaskModel _model;
        public FaxEmailModel _model1;
        public MedicalAuthModel _Model = new MedicalAuthModel();
        ManagedRxModel managedRxModel = new ManagedRxModel();
        public const int MSG_OPEN_ATTACH_DOC_POPUP = 105;
        public const int MSG_OPEN_FAX_EMAIL_POPUP = 106;
        public string CoverLetter { get; set; }

        private static string ApplicatioMode = ConfigurationManager.AppSettings["ApplicationMode"] ?? "";
        string[] InteropPDFdllServers = Convert.ToString(ConfigurationManager.AppSettings["InteropPDFdll_Servers"] ?? "").Split(',');
        private string TempCoverFaxesPath = Convert.ToString(ConfigurationManager.AppSettings["TEMP_COVER_FAXES_PATH"]);
        //private string TempCoverFaxesPath1 = Convert.ToString(ConfigurationManager.AppSettings["TEMP_COVER_FAXES_PATH1"]);
        private string TempCoverFaxesPath1 = Convert.ToString(ConfigurationManager.AppSettings["FAX_NUMBER"]);
        private string TempCoverFaxesPath2 = Convert.ToString(ConfigurationManager.AppSettings["FAX_NUMBER_HMOI"]);
        string NEWWin_Logo = ConfigurationManager.AppSettings["NEWWin_Logo"].ToString();

        private static readonly List<string> FaxEnabledPharmacies = new List<string>
        {
            "AlexanderTwin Pharmacy",
            "Alto Pharmacy",
            "Amber Specialty Pharmacy",
            "Apri Fertility Pharmacy",
            "Apthorp Pharmacy",
            "CVS Specialty Pharmacy",
            "Dobbs Ferry Pharmacy",
            "Fertility Pharmacy of America",
            "Freedom Fertility by Evernorth",
            "Genoa Healthcare / Genoa Fertility Pharmacy",
            "Hann's Fertility Pharmacy",
            "HealthDyne Specialty Pharmacy",
            "Integrity Rx Specialty Pharmacy",
            "Mandell's Clinical Pharmacy",
            "MDR Specialty Pharmacy / MDRX",
            "Omaha Pharmacy Express",
            "Prima Pharmacy",
            "SandsRx",
            "SMP Pharmacy Solutions",
            "Village Fertility Pharmacy"
        };
        //private List<string> PREMIER = new List<string>() { "EPIC Brokers", "Disney-HMSA Kaiser", "Davis Wright Tremaine LLP", "JP Morgan-No DOI-Premier", "DE Shaw-US",
        //    "DE Shaw-Fully Insured", "General Atlantic","Legal Aid Society", "Freshfields", "First American-HMSA", "Bill and Melinda Gates Companies", "PGA TOUR",
        //    "Disney-21 Century Fox-Kaiser","Disney-Hulu-Kaiser","JPMS-John Paul Mitchell Systems", "MCI-Premier", "Loeb & Loeb-Cigna-No Medical Election-Premier",
        //    "Loeb & Loeb-Oxford-Premier","Pivotal Ventures-Premier","Mente-Premier","Televisa Univision-Premier","Cascade Asset Management-Premier","Gates Archives-Premier","Watermark-Premier",
        //    "Breakthrough Energy-Premier","Gates Ventures-Premier","Global Health Labs-Premier","Evercore-No Medical Election","Evercore-Aetna-Premier",
        //    "Ayzenberg Group-Premier","Kantar-Premier","Kantar - No Medical Election","Chewy-Aetna-Premier","Chewy-No Medical Election",
        //    "NARAL Pro-Choice America-Premier","AV Squad-Premier","Cambrex-Premier","Munger, Tolles & Olson-Premier","Ameritas - No Medical Election",
        //    "LMI-No Medical Election","Friedkin Group-No Medical Election","Bath & Body Works-No Medical Election","SPARC Group (Aeropostale, Brooks Brothers, Lucky Brand, Eddie Bauer, Forever 21, Nautica, Reebok)-No Medical Election",
        //    "Memorial Sloan Kettering (MSK)-Aetna-Premier","Memorial Sloan Kettering (MSK)-UHC-Premier","Memorial Sloan Kettering (MSK)-Anthem-Premier","Gilbane-Premier-No DOI","LMI-Premier",
        //    "Littler Mednelson-Premier","Cboe-BCBSIL-Premier","Autonation-Premier","Ameritas - UMR - Premier","Friedkin Group-Aetna-Premier","Friedkin Group-Ameriben-Premier",
        //    "SPARC Group (Aeropostale, Brooks Brothers, Lucky Brand, Eddie Bauer, Forever 21, Nautica, Reebok)-Kaiser-Premier","Bath & Body Works-HMSA, Puerto Rico-Premier",
        //    "Duane Morris-Premier","Wipfli LLP-UMR-Premier","Wipfli LLP-Kaiser No Medical Election","Bath & Body Works-HMSA, Puerto Rico, & No Medical Election","Gilbane-BCBSRI-Premier",
        //    "Autonation-Highmark-Premier","Littler Mendelson-Premier","LMI-UHC-Premier","Littler Mendelson-UHC-Premier","Autonation-No Medical Election","Littler Mendelson-No Medical Election", "Cboe-Premier","SIG Susquehanna-Aetna-Premier",
        //"Kimco Realty-Trustmark-Premier","WPP-Aetna-Premier","WPP-UHC-Premier","WPP-All Other Plans","Televisa Univision-No Medical Election"};

        #endregion

        #region constructor  

        public AuditApprovalTaskVM()
        {
            this.Model = new AuditApprovalTaskModel();
            this._model1 = new FaxEmailModel();
            this.IsButtonEnabled = true;

        }

        #endregion

        #region properties

        //private AuditApprovedPrescriptionDetails _auditApprovedPrescriptionDetails;
        //public AuditApprovedPrescriptionDetails AuditApprovedPrescriptionDetails
        //{
        //    get { return this._auditApprovedPrescriptionDetails; }
        //    set { this._auditApprovedPrescriptionDetails = value; this.Notify(nameof(this.AuditApprovedPrescriptionDetails)); }
        //}
        private List<string> _disabledcontrols;
        public List<string> DisabledControls
        {
            get { return this._disabledcontrols; }
            set { this._disabledcontrols = value; this.Notify(nameof(this.DisabledControls)); }
        }


        private FaxProvider _searchOutput = new FaxProvider();
        public FaxProvider SearchOutput
        {
            get { return this._searchOutput; }
            set { this._searchOutput = value; Notify(nameof(this.SearchOutput)); }
        }
        private Boolean _isFaxEnabled;
        public Boolean IsFaxEnabled
        {
            get { return this._isEnabledFax; }
            set { this._isEnabledFax = value; Notify(nameof(this.IsFaxEnabled)); }
        }

        private PopulateLetterDetails _selectedInputItem = new PopulateLetterDetails();
        public PopulateLetterDetails SelectedInputItem
        {
            get { return this._selectedInputItem; }
            set
            {
                this._selectedInputItem = value; Notify(nameof(this.SelectedInputItem));
                if (this.SelectedInputItem != null)
                {
                    SearchOutput.FaxDetails.AuthEmail = this.SelectedInputItem.Email;
                    SearchOutput.FaxDetails.AuthFaxNumber = this.SelectedInputItem.Fax;
                    if (this.SelectedInputItem.Email == "")
                    {
                        this.IsFaxEnabled = true;

                    }

                }
            }
        }

        private List<ObjectBase> _tasktolist;

        public List<ObjectBase> TaskToList
        {
            get { return _tasktolist; }
            set { _tasktolist = value; this.Notify(nameof(this.TaskToList)); }
        }
        private List<ObjectBase> _pharmacylist;
        public List<ObjectBase> PharmacyList
        {
            get { return _pharmacylist; }
            set { _pharmacylist = value; this.Notify(nameof(this.PharmacyList)); }
        }

        public string PngDocpath { get; set; }
        public string TempPath { get; set; }
        public string PngPath { get; set; }


        private Alerts _alerts = new Alerts();
        public Alerts Alerts
        {
            get { return _alerts; }
            set { _alerts = value; }
        }

        private bool _isVisibleAlerts;
        private bool _isEnabledFax;

        public bool IsVisibleAlerts
        {
            get { return this._isVisibleAlerts; }
            set { this._isVisibleAlerts = value; this.Notify(nameof(this.IsVisibleAlerts)); }
        }

        private bool _isButtonEnabled;

        public bool IsButtonEnabled
        {
            get { return this._isButtonEnabled; }
            set { this._isButtonEnabled = value; this.Notify(nameof(this.IsButtonEnabled)); }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        #endregion
        public string Convertpngtopdf()
        {
            try
            {
                // creation of the document with a certain size and certain margins
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0);
                PngPath = TempPath + "\\" + "TempDoc.pdf";
                //if (File.Exists(PngPath))
                //{
                //    File.Delete(PngPath);
                //}
                // creation of the different writers  
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new System.IO.FileStream(PngPath, System.IO.FileMode.Create));
                // load the tiff image and count the total pages             
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(PngDocpath);
                int total = bm.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
                document.Open();
                iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
                for (int k = 0; k < total; ++k)
                {
                    bm.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bm, System.Drawing.Imaging.ImageFormat.Bmp);
                    // scale the image to fit in the page  
                    img.ScalePercent(72f / img.DpiX * 46);
                    img.SetAbsolutePosition(0, 0);
                    cb.AddImage(img);
                    document.NewPage();
                }
                document.Close();
                return PngPath;
            }
            catch (Exception ex)
            { throw ex; }
        }
        #region Commands

        public ICommand SearchCommand
        {
            get
            {
                return new ATSCommand(new Action<object>((args) =>
                {
                    try
                    {
                        if (args != null && args is string)
                        {
                            string flag = args.ToString();

                            if (string.Compare(flag, "Attach", true) == 0)
                            {
                                try
                                {
                                    if (this.View is IWindowMsg)
                                    {
                                        var msgWindow = this.View as IWindowMsg;
                                        var result = msgWindow.Send(MSG_OPEN_ATTACH_DOC_POPUP, new AttachmentsParams()
                                        {
                                            Contract = Data.ShippingDetails.Contract ?? "",
                                            MemberID = Data.ShippingDetails.MemberId ?? "",
                                            DocumentTypeParam = "Prescription"
                                        });
                                        if (result != null)
                                        {
                                            this.Attachments = result as List<ObjectBase>;
                                            if (Data.NextTask.Attachments == null)
                                            {
                                                Data.NextTask.Attachments = new List<ObjectBase>();
                                            }
                                            Data.NextTask.Attachments = Data.NextTask.Attachments.Concat(this.Attachments).ToList();
                                        }


                                    }
                                }
                                catch { }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An exception captured " + ex.Message);
                    }
                }));
            }
        }

        private bool IsFaxEnabledPharmacy(string pharmacyName)
        {
            if (string.IsNullOrWhiteSpace(pharmacyName))
                return false;

            string contract = Data?.ScreenArgs?.Contract ?? Data?.AuthHeaderDetails?.Contract ?? "";

            // Only for Ally contract → new pharmacies list
            if (string.Equals(contract, "Ally", StringComparison.OrdinalIgnoreCase))
            {
                return FaxEnabledPharmacies.Any(p =>
                    pharmacyName.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Old logic for all other contracts (CVS / CAREMARK only)
            return pharmacyName.IndexOf("CVS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   pharmacyName.IndexOf("CAREMARK", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string GetPharmacyFaxNumber(string pharmacyName)
        {
            if (string.IsNullOrWhiteSpace(pharmacyName))
                return ConfigurationManager.AppSettings["FAX_NUMBER"] ?? "";

            string contract = Data?.ScreenArgs?.Contract ?? Data?.AuthHeaderDetails?.Contract ?? "";

            // Only for Ally → dynamic fax numbers
            if (string.Equals(contract, "Ally", StringComparison.OrdinalIgnoreCase))
            {
                pharmacyName = pharmacyName.ToUpper();

                if (pharmacyName.Contains("ALEXANDERTWIN"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_ALEXANDERTWIN"] ?? "";
                if (pharmacyName.Contains("ALTO"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_ALTO"] ?? "";
                if (pharmacyName.Contains("AMBER"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_AMBER"] ?? "";
                if (pharmacyName.Contains("APRI"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_APRI"] ?? "";
                if (pharmacyName.Contains("APTHORP"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_APTHORP"] ?? "";
                if (pharmacyName.Contains("CVS") || pharmacyName.Contains("CAREMARK"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_CVS"] ?? "";
                if (pharmacyName.Contains("DOBBS FERRY") || pharmacyName.Contains("DOBBSFERRY"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_DOBBSFERRY"] ?? "";
                if (pharmacyName.Contains("FERTILITY PHARMACY OF AMERICA"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_FERTILITYPHARMACY"] ?? "";
                if (pharmacyName.Contains("FREEDOM"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_FREEDOM"] ?? "";
                if (pharmacyName.Contains("GENOA"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_GENOA"] ?? "";
                if (pharmacyName.Contains("HANN"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_HANNS"] ?? "";
                if (pharmacyName.Contains("HEALTHDYNE"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_HEALTHDYNE"] ?? "";
                if (pharmacyName.Contains("INTEGRITY"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_INTEGRITY"] ?? "";
                if (pharmacyName.Contains("MANDELL"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_MANDELLS"] ?? "";
                if (pharmacyName.Contains("MDR"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_MDR"] ?? "";
                if (pharmacyName.Contains("OMAHA"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_OMAHA"] ?? "";
                if (pharmacyName.Contains("PRIMA"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_PRIMA"] ?? "";
                if (pharmacyName.Contains("SANDSRX") || pharmacyName.Contains("SANDS"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_SANDSRX"] ?? "";
                if (pharmacyName.Contains("SMP"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_SMP"] ?? "";
                if (pharmacyName.Contains("VILLAGE"))
                    return ConfigurationManager.AppSettings["FAX_NUMBER_VILLAGE"] ?? "";
            }

            // Old behavior for all other contracts
            return ConfigurationManager.AppSettings["FAX_NUMBER"] ?? "";
        }

        #endregion

        #region Methods

        protected override void OnPreInit(TaskScreenArgs args)
        {
            /*----- Task Priority List -----*/
            this.PriorityList = new MedicalAuthModel().GetPriorityList("AuditApprovalTaskVM");
        }
        private ForwardEmailDetails _faxemaidata = new ForwardEmailDetails();
        public ForwardEmailDetails FaxEmailData
        {
            get { return this._faxemaidata; }
            set { this._faxemaidata = value; this.Notify(nameof(this.FaxEmailData)); }
        }

        protected override bool OnPreExecuteCommand(object args)
        {

            //this.Data.ShippingDetails.MailBody = "";
            //this.Data.ShippingDetails.MailFooter = "";
            //if (this.Attachments.Count > 0)
            //{
            //    this.Data.NextTask.Attachments = this.Attachments;
            //}
            if (args.ToString().Equals("SAVE"))
                this.IsButtonEnabled = false;
            return true;


        }

        private string PreviewDoc(string Sourcepath, string Targetpath)
        {
            string temp = string.Empty;
            try
            {
                string addressdet = SelectedInputItem?.ProAddress1 ?? "";
                List<string> names = addressdet.Split(',').ToList<string>();

                TemplateDataRow = this._model1.GetTemplateData(this.RequestBase);
                if (TemplateDataRow.Count == 0)
                {
                    TemplateDataRow["TF_TODAY"] = DateTime.Now.ToShortDateString();
                    TemplateDataRow["TF_PROVIDER_NAME"] = (Data.ShippingDetails.PhysicianName ?? "");
                    TemplateDataRow["TF_PROVIDER_FAX"] = (Data.AuthHeaderDetails.Fax ?? "");
                    TemplateDataRow["TF_PROVIDER_PHONE"] = ("" ?? "");
                    TemplateDataRow["TF_MEMBER_NAME"] = (Data.ShippingDetails.PatientName ?? "");
                    TemplateDataRow["TF_MEMBER_ID"] = (Data.AuthHeaderDetails.MemberId ?? "");
                    TemplateDataRow["TF_PROVIDER_GROUP_NAME"] = (Data.ShippingDetails.GroupName ?? "");
                    TemplateDataRow["TF_MEMBER_GROUP_NAME"] = (SelectedInputItem?.MemberGroupID ?? "");

                    TemplateDataRow["TF_MEMBER_DOB"] = (Data.ShippingDetails.DateOfBirth ?? "");
                    if (names.Count >= 1) { TemplateDataRow["TF_PROVIDER_CITY"] = (names[0].ToString() ?? ""); }
                    if (names.Count >= 2) { TemplateDataRow["TF_PROVIDER_STATE"] = (names[1].ToString() ?? ""); }
                }
                TemplateDataRow.Add("TF_DYNAMIC_TEXT", this.SearchOutput.FaxDetails.AuthFaxNumber ?? "");
                TemplateDataRow["TF_PROVIDER_FAX"] = GetPharmacyFaxNumber(Data.ShippingDetails.PharmacyName);


                try
                {
                    string TemplateValue = TemplateDataRow["TF_ANTHEMSUBCODE"];
                    List<string> listofAnthemCode = TemplateValue.Split('^').ToList<string>();
                    string finalAnthemlist = string.Empty;
                    foreach (var item in listofAnthemCode)
                    {
                        finalAnthemlist += item + Environment.NewLine;
                    }
                    TemplateDataRow["TF_ANTHEMSUBCODE"] = finalAnthemlist;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception captured " + ex.Message);
                }

                string datefile = DateTime.Now.ToString("HH-mm-ss");
                int j = 0;
                string Filename = Sourcepath;
                j = Filename.LastIndexOf("\\");
                Filename = Filename.Substring(j + 1);

                bool folderexists = System.IO.Directory.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + "Doc");
                if (folderexists)
                {
                    temp = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + "Doc" + "\\" + Filename.Replace(".docx", ".docx");
                }
                else
                {
                    System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + "Doc");
                    temp = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + "Doc" + "\\" + Filename.Replace(".docx", ".docx");
                }
                System.IO.File.Copy(Sourcepath, temp, true);
                System.IO.File.Copy(temp, Targetpath, true);
                TemplateMergeUtils.RunForTagReplace(temp, Targetpath, TemplateDataRow);
                //if (System.IO.File.Exists(temp) == true)
                //{
                //    System.IO.File.Delete(temp);
                //}
            }
            catch (Exception ex)
            {
                //this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Unable to preview the document. Please contact IT Support.");
                return "";
            }
            return temp;
        }

        private void SendFax(string targetpath, string faxid, string mode = null, string FaxTriggerMode = null)
        {
            try
            {
                RequestBase input = new RequestBase()
                {
                    AuthNo = Data.ScreenArgs.AuthNumber ?? "",
                    Contract = Data.AuthHeaderDetails.Contract ?? "",
                    MemberID = Data.AuthHeaderDetails.MemberId ?? ""
                };



                if (this.Data.ShippingDetails.PatientName.IndexOf("TEST", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    FaxEmailData.Status = "Fax";
                    FaxEmailData.Contract = Data.AuthHeaderDetails.Contract ?? "";
                    FaxEmailData.FaxId = faxid ?? "";
                    FaxEmailData.Fax = TempCoverFaxesPath2 ?? "";
                    FaxEmailData.Memberid = Data.AuthHeaderDetails.MemberId ?? "";
                    FaxEmailData.UserName = Utilities.Common.UserName ?? "";

                    FaxEmailData.DocPath = targetpath ?? "";
                    FaxEmailData.Attachments.Add(new MailAttachment() { DisplayName = "Authorization", FilePath = FaxEmailData.DocPath, FaxID = faxid });
                    FaxEmailData.FaxId = faxid ?? "";
                    MemberSnapshotModel _snapshotmodel = new MemberSnapshotModel();
                    string res = _snapshotmodel.SendFaxMail(FaxEmailData);

                }
                else
                {
                    FaxEmailData.Status = "Fax";
                    FaxEmailData.Contract = Data.AuthHeaderDetails.Contract ?? "";
                    FaxEmailData.FaxId = faxid ?? "";
                    FaxEmailData.Fax = GetPharmacyFaxNumber(Data.ShippingDetails.PharmacyName);
                    FaxEmailData.Memberid = Data.AuthHeaderDetails.MemberId ?? "";
                    FaxEmailData.UserName = Utilities.Common.UserName ?? "";

                    FaxEmailData.DocPath = targetpath ?? "";
                    FaxEmailData.Attachments.Add(new MailAttachment() { DisplayName = "Authorization", FilePath = FaxEmailData.DocPath, FaxID = faxid });
                    FaxEmailData.FaxId = faxid ?? "";
                    MemberSnapshotModel _snapshotmodel = new MemberSnapshotModel();
                    string res = _snapshotmodel.SendFaxMail(FaxEmailData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception captured " + ex.Message);
            }
        }
        private FaxProvider _outputResult = new FaxProvider();
        public FaxProvider OutputResult
        {
            get { return this._outputResult; }
            set { this._outputResult = value; Notify(nameof(this.OutputResult)); }
        }

        private string _docPath;
        public string gbl_DocPath
        {
            get { return this._docPath; }
            set { this._docPath = value; this.Notify(nameof(this.gbl_DocPath)); }
        }

        private ObjectBase PopulateData(bool isPrescription = false)
        {
            string Sourcepath = string.Empty;
            string Targetpath = string.Empty;

            try
            {
                AuthList args = new AuthList()
                {
                    AuthNo = Data.AuthHeaderDetails.AuthNo ?? "",
                    Contract = Data.AuthHeaderDetails.Contract ?? "",
                    Coverletter = this.CoverLetter ?? "",
                    CignaFax = ""
                };

                string DocPath = string.Empty;
                //if ((this.gbl_DocPath ?? "") != "")
                //{ DocPath = this.gbl_DocPath; }
                //else { this.OutputResult = this._model1.PopulateTemplate(args); }
                if (this.Data.NextTask.Attachments.Count > 0)
                {
                    foreach (var data in this.Data.NextTask.Attachments.ToList())
                    {
                        if ((data.Additional == "Prescription" || data.Tag.Contains("Prescription")) && (isPrescription == false))
                        {
                            try
                            {
                                string datefile = DateTime.Now.ToString("HH-mm-ss");
                                DocPath = string.Empty;
                                if (this.RequestBase.TemplatePath != "" && this.RequestBase.TemplatePath != null)
                                { DocPath = data.Tag; }
                                else if ((gbl_DocPath ?? "") != "") { DocPath = data.Tag; }
                                else { DocPath = data.Tag; }
                                int i = 0;
                                string Filename = DocPath;
                                i = Filename.LastIndexOf("\\");
                                Filename = Filename.Substring(i + 1);
                                if (System.IO.File.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" + "docx" + "\\Tempxml2DocATS.docx"))
                                {
                                    Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" + "docx" + "\\Tempxml2DocATS.docx";
                                    Sourcepath = DocPath;
                                }
                                else if (System.IO.File.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" + "docx" + "\\" + Filename.Replace(".docx", ".docx")))
                                {
                                    Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" + "docx" + "\\" + Filename.Replace(".docx", ".docx");
                                    Sourcepath = DocPath;
                                }
                                else
                                {
                                    bool exists = System.IO.Directory.Exists(TempCoverFaxesPath + "\\" +
                                        Utilities.Common.UserFullName + "\\" + Contract + "\\" +
                                        Data.AuthHeaderDetails.MemberId + "\\" + "docx");

                                    if (exists)
                                    {
                                        Directory.Delete(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" +
                                            Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" +
                                            "docx", true);

                                        System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" +
                                            Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" +
                                            Data.AuthHeaderDetails.MemberId + "\\" + "docx");

                                        Sourcepath = DocPath ?? "";
                                        string extension = Path.GetExtension(Sourcepath);
                                        Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" +
                                            Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" +
                                            "docx" + "\\" + Filename.Replace(".docx", ".docx");

                                        var filename = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" +
                                            Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" +
                                            "docx" + "\\Tempxml2DocATS" + extension;

                                        FileInfo fi = new FileInfo(Sourcepath);
                                        fi.CopyTo(Targetpath);
                                    }
                                    else
                                    {
                                        System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" +
                                            Utilities.Common.UserFullName + "\\" + Data.AuthHeaderDetails.Contract + "\\" +
                                            Data.AuthHeaderDetails.MemberId + "\\" + "docx");

                                        Sourcepath = DocPath ?? "";
                                        string extension = Path.GetExtension(Sourcepath);
                                        Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" +
                                            Data.AuthHeaderDetails.Contract + "\\" + Data.AuthHeaderDetails.MemberId + "\\" +
                                            "docx" + "\\" + Filename.Replace(".docx", ".docx");

                                        FileInfo fi = new FileInfo(Sourcepath);
                                        fi.CopyTo(Targetpath ?? "");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An exception captured " + ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                if ((this.CoverLetter ?? "") == "") { this.CoverLetter = string.Compare("Approval", RequestBase.AuthStatus, true) == 0 ? "No" : string.Compare("Pending", RequestBase.AuthStatus, true) == 0 ? "GenerateLetter" : "Yes"; }
                                FaxEmailModel model1 = new FaxEmailModel();
                                if (!string.IsNullOrEmpty(this.gbl_DocPath))
                                {
                                    DocPath = this.gbl_DocPath;
                                    this.OutputResult = model1.PopulateTemplate(args);
                                }
                                else
                                {
                                    this.OutputResult = model1.PopulateTemplate(args);
                                }

                                if (!string.IsNullOrEmpty(DocPath))
                                {
                                    int i = 0;
                                    string Filename = DocPath;
                                    i = Filename.LastIndexOf("\\");
                                    Filename = Filename.Substring(i + 1);
                                    try
                                    {
                                        if (System.IO.File.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover" + "\\Tempxml2DocATS.docx"))
                                        {
                                            string extension = string.Empty;
                                            extension = Path.GetExtension(DocPath);
                                            Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover" + "\\Tempxml2DocATS" + extension;
                                            Sourcepath = DocPath;
                                        }
                                        else
                                        {
                                            bool exists = System.IO.Directory.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover");
                                            if (exists)
                                            {
                                                Directory.Delete(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover", true);
                                                System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover");
                                                Sourcepath = DocPath;
                                                string extension = string.Empty;
                                                extension = Path.GetExtension(DocPath);
                                                Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover" + "\\Tempxml2DocATS" + extension;
                                                FileInfo fi = new FileInfo(Sourcepath);
                                                fi.CopyTo(Targetpath);
                                                fi = null;
                                            }
                                            else
                                            {
                                                System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover");
                                                Sourcepath = DocPath;
                                                string extension = string.Empty;
                                                extension = Path.GetExtension(DocPath);
                                                Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "Cover" + "\\Tempxml2DocATS" + extension;
                                                FileInfo fi = new FileInfo(Sourcepath);
                                                fi.CopyTo(Targetpath);
                                                fi = null;
                                            }
                                        }
                                    }
                                    catch (Exception) { }
                                }
                                else
                                {
                                    if ((this.OutputResult.FaxPopulate.TemplatePath != "" && this.OutputResult.FaxPopulate.TemplatePath != null) ||
                                        (this.RequestBase.TemplatePath != "" && this.RequestBase.TemplatePath != null))
                                    {
                                        try
                                        {
                                            string datefile = DateTime.Now.ToString("HH-mm-ss");
                                            DocPath = string.Empty;
                                            if (this.RequestBase.TemplatePath != "" && this.RequestBase.TemplatePath != null)
                                            { DocPath = this.RequestBase.TemplatePath; }
                                            else if ((gbl_DocPath ?? "") != "") { DocPath = gbl_DocPath; }
                                            else { DocPath = this.OutputResult.FaxPopulate.TemplatePath; }
                                            int i = 0;
                                            string Filename = DocPath;
                                            i = Filename.LastIndexOf("\\");
                                            Filename = Filename.Substring(i + 1);
                                            if (System.IO.File.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\Tempxml2DocATS.docx"))
                                            {
                                                Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\Tempxml2DocATS.docx";
                                                Sourcepath = DocPath;
                                            }
                                            else if (System.IO.File.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\" + Filename.Replace(".docx", ".docx")))
                                            {
                                                Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\" + Filename.Replace(".docx", ".docx");
                                                Sourcepath = DocPath;
                                            }
                                            else
                                            {
                                                bool exists = System.IO.Directory.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx");
                                                if (exists)
                                                {
                                                    Directory.Delete(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx", true);
                                                    System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx");
                                                    Sourcepath = DocPath ?? "";
                                                    string extension;
                                                    Sourcepath = DocPath ?? "";
                                                    extension = Path.GetExtension(Sourcepath);
                                                    Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\" + Filename.Replace(".docx", ".docx");
                                                    var filename = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\Tempxml2DocATS" + extension;
                                                    FileInfo fi = new FileInfo(Sourcepath);
                                                    fi.CopyTo(Targetpath ?? "");
                                                    fi = null;
                                                }
                                                else
                                                {
                                                    System.IO.Directory.CreateDirectory(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx");
                                                    string extension;
                                                    Sourcepath = DocPath ?? "";
                                                    extension = Path.GetExtension(Sourcepath ?? "");
                                                    Targetpath = TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName + "\\" + Contract + "\\" + this.RequestBase.MemberID + "\\" + "docx" + "\\Tempxml2DocATS" + extension;
                                                    FileInfo fi = new FileInfo(Sourcepath ?? "");
                                                    fi.CopyTo(Targetpath ?? "");
                                                    fi = null;
                                                }
                                            }
                                        }
                                        catch (Exception e) { }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Unable to populate the data. Please contact IT Support.");
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception captured " + ex.Message);
            }
            return new ObjectBase() { ID = Sourcepath, Text = Targetpath };
        }
        protected override void OnPostExecuteCommand(RequestBase args)
        {
            bool show = true;
            var value = this.Data as AuditApprovedPrescriptionDetails;
            this.IsButtonEnabled = true;
            if (value.Faxflag == "1")
            {
                if (args.ToString() == "SAVE")
                {
                    if (this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Confirmation, "Do you wish to Fax/Email/Notify the provider ?", new string[] { "Yes", "No" }) != 0)
                    {
                        show = false;
                        // Generateletter();
                    }
                    if (show == true)
                    {
                        try
                        {
                            if (this.View is IWindowMsg)
                            {
                                var msgWindow = base.IUIWindow as IWindowMsg;

                                var result = (RequestBase)msgWindow.Send(MemberSnapshotVM.POPUP_FOR_FAXEMAIL, new RequestBase()
                                {
                                    Contract = Data.AuthHeaderDetails.Contract ?? "",
                                    MemberID = Data.AuthHeaderDetails.MemberId ?? "",
                                    AuthNo = Data.ScreenArgs.AuthNumber ?? "",
                                    FormatAuthNo = Data.ScreenArgs.FormattedAuthNumber ?? "",
                                    OutPatientId = Data.ScreenArgs.OutPatientID ?? "",
                                    AAPTaskid = Data.ScreenArgs.TaskID ?? "",
                                    Type = "AAP",
                                });
                                //Generateletter();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An exception captured " + ex.Message);
                        }
                    }

                }

            }

            if (this.Data.NextTask.Attachments.Count > 0)
            {
                foreach (var data in this.Data.NextTask.Attachments.ToList())
                {
                    if (IsFaxEnabledPharmacy(Data.ShippingDetails.PharmacyName) &&
                        (data.Additional == "Prescription" || data.Tag.IndexOf("Prescription", StringComparison.OrdinalIgnoreCase) >= 0) &&
                        Data.ShippingDetails.Status.Text == "Complete")
                    {
                        string deletepath = string.Empty;
                        ObjectBase obj = new ObjectBase();
                        obj = PopulateData();
                        string FaxTriggerMode = string.Empty;
                        FaxTriggerMode = "Fax Immediate";
                        string res = string.Empty;
                        new Thread(new ThreadStart(() =>
                        {
                            try
                            {
                                res = PreviewDoc(obj.ID, obj.Text);
                                // obj.Text = Data.ShippingDetails.TemplatePath;
                                string Temp = string.Empty;
                                string PngExttension = Path.GetExtension(obj.Text);
                                if (InteropPDFdllServers.Contains(System.Net.Dns.GetHostName().ToString().ToUpper()))
                                {
                                    Temp = Utilities.ConvertDocument.ConverttoPDF("", obj.Text);
                                }

                                else if (string.Equals(PngExttension, ".JPG", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".JPEG", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".JPE", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".PNG", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".JIF", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".JFIF", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".JFI", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".TIFF", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(PngExttension, ".TIF", StringComparison.OrdinalIgnoreCase))
                                {
                                    this.PngDocpath = obj.Text;
                                    try { TempPath = System.Configuration.ConfigurationManager.AppSettings["TEMP_SAVE_DOC_PATH"]; } catch { }
                                    string path = Convertpngtopdf();
                                    int cont = path.LastIndexOf("\\");
                                    obj.Text = Data.ShippingDetails.Contract + "\\" + path.Substring(cont + 1);
                                    obj.Text = path;
                                    Temp = Utilities.ConvertDocument.WordToPDF(obj.Text);
                                }
                                else
                                {

                                    Temp = Utilities.ConvertDocument.WordToPDF(obj.Text);
                                }

                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    try
                                    {
                                        int j = 0;
                                        string NoofPages = Temp;
                                        j = Temp.LastIndexOf(",");
                                        NoofPages = NoofPages.Substring(j + 1);

                                        FaxLog objfaxlog = new FaxLog()
                                        {
                                            Contract = Data.AuthHeaderDetails.Contract ?? "",
                                            Speciality = "Infertility",
                                            MemberID = Data.AuthHeaderDetails.MemberId ?? "",
                                            LastUser = Utilities.Common.UserName,
                                            AuthNo = Data.ScreenArgs.AuthNumber ?? "",
                                            NoOfPages = NoofPages,
                                            OutID = Data.ScreenArgs.OutPatientID ?? "",
                                            ServiceProID = this.SelectedInputItem?.ServiceProID ?? "",
                                            GroupId = this.SelectedInputItem?.GroupID ?? "",
                                            Notes = "AuthNo:" + Data.ScreenArgs.AuthNumber + Environment.NewLine + $"Fax has been sent to {Data.ShippingDetails.PharmacyName}",
                                            StatusDesc = "Pharmacy Fax",
                                            DocTypeDesc = this.RequestBase?.Type ?? "Authorization",
                                            LetterType = "",
                                            FunnctionalArea = "",
                                            LetterTypeId = this.RequestBase?.LetterTypeId ?? "",
                                            TaskID = this.RequestBase?.AAPTaskid ?? ""
                                        };
                                        this.InsertFaxLogFileResults = _model1.InsertFaxLogFile(objfaxlog);


                                        if ((this.InsertFaxLogFileResults?.Text ?? "") != "")
                                        {
                                            string TifFolder = string.Empty;
                                            string[] tifpath = InsertFaxLogFileResults.Text.Split('\\');
                                            if (tifpath[1].Length > 0)
                                            {
                                                TifFolder = "\\\\" + tifpath[1] + "\\" + tifpath[2] + "\\" + tifpath[3] + "\\" + tifpath[4];
                                            }
                                            else if (tifpath[2].Length > 0)
                                            {
                                                TifFolder = "\\\\" + tifpath[2] + "\\" + tifpath[3] + "\\" + tifpath[4];
                                            }
                                            else if (tifpath[3].Length > 0)
                                            {
                                                TifFolder = "\\\\" + tifpath[3] + "\\" + tifpath[4];
                                            }
                                            for (int k = 5; k <= tifpath.Length - 1; k++)
                                            {
                                                TifFolder = TifFolder + "\\" + tifpath[k];
                                                if (!(System.IO.Directory.Exists(TifFolder)))
                                                {
                                                    System.IO.Directory.CreateDirectory(TifFolder);
                                                }
                                            }


                                            Thread.Sleep(300);
                                            string extension = Path.GetExtension(InsertFaxLogFileResults.Additional);

                                            if (extension == ".pdf")
                                            {
                                                deletepath = obj.Text ?? "";
                                                if (obj.Text.EndsWith(".doc")) { obj.Text = obj.Text.Replace(".doc", ".pdf"); }
                                                else if (obj.Text.EndsWith(".docx")) { obj.Text = obj.Text.Replace(".docx", ".pdf"); }
                                            }

                                            string targetpath = obj.Text ?? "";
                                            FileInfo fi = new FileInfo(obj.Text);
                                            fi.CopyTo(this.InsertFaxLogFileResults.Additional);
                                            fi = null;
                                        }

                                        SendFax(this.InsertFaxLogFileResults.Additional, this.InsertFaxLogFileResults.ID, RequestBase.Mode, FaxTriggerMode);


                                        //bool exists = System.IO.Directory.Exists(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName);
                                        //if (exists)
                                        //{
                                        //    Directory.Delete(TempCoverFaxesPath + "\\" + Utilities.Common.UserFullName, true);
                                        //}
                                    }
                                    catch { Exception ex; }

                                }), System.Windows.Threading.DispatcherPriority.Send);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An exception captured " + ex.Message);
                            }
                            finally
                            {
                                try
                                {
                                    bool exists = System.IO.File.Exists(deletepath);
                                    if (exists)
                                    {
                                        System.IO.File.Delete(deletepath);
                                    }

                                    bool exists1 = System.IO.File.Exists(obj.Text);
                                    if (exists1)
                                    {
                                        System.IO.File.Delete(obj.Text ?? "");
                                    }
                                    if (System.IO.File.Exists(res ?? ""))
                                    {
                                        System.IO.File.Delete(res ?? "");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An exception captured " + ex.Message);
                                }
                            }
                        })).Start();

                        this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Information, $"Fax has been sent to {Data.ShippingDetails.PharmacyName}.");
                    }
                }
            }
            if (IsFaxEnabledPharmacy(Data.ShippingDetails.PharmacyName) &&
                                  (this.Data.ShippingDetails.Status.Text == "Complete"))
            {

                string deletepath = string.Empty;
                ObjectBase obj = new ObjectBase();
                obj = PopulateData(true);
                string FaxTriggerMode = string.Empty;
                FaxTriggerMode = "Fax Immediate";
                string res = string.Empty;
                new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        res = PreviewDoc(obj.ID, obj.Text);
                        // obj.Text = Data.ShippingDetails.TemplatePath;
                        string Temp = string.Empty;
                        string PngExttension = Path.GetExtension(obj.Text);
                        if (InteropPDFdllServers.Contains(System.Net.Dns.GetHostName().ToString().ToUpper()))
                        {
                            Temp = Utilities.ConvertDocument.ConverttoPDF("", obj.Text);
                        }

                        else if (string.Equals(PngExttension, ".JPG", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".JPEG", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".JPE", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".PNG", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".JIF", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".JFIF", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".JFI", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".TIFF", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(PngExttension, ".TIF", StringComparison.OrdinalIgnoreCase))
                        {
                            this.PngDocpath = obj.Text;
                            try { TempPath = System.Configuration.ConfigurationManager.AppSettings["TEMP_SAVE_DOC_PATH"]; } catch { }
                            string path = Convertpngtopdf();
                            int cont = path.LastIndexOf("\\");
                            obj.Text = Data.ShippingDetails.Contract + "\\" + path.Substring(cont + 1);
                            obj.Text = path;
                            Temp = Utilities.ConvertDocument.WordToPDF(obj.Text);
                        }
                        else
                        {

                            Temp = Utilities.ConvertDocument.WordToPDF(obj.Text);
                        }

                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                int j = 0;
                                string NoofPages = Temp;
                                j = Temp.LastIndexOf(",");
                                NoofPages = NoofPages.Substring(j + 1);

                                FaxLog objfaxlog = new FaxLog()
                                {
                                    Contract = Data.AuthHeaderDetails.Contract ?? "",
                                    Speciality = "Infertility",
                                    MemberID = Data.AuthHeaderDetails.MemberId ?? "",
                                    LastUser = Utilities.Common.UserName,
                                    AuthNo = Data.ScreenArgs.AuthNumber ?? "",
                                    NoOfPages = NoofPages,
                                    OutID = Data.ScreenArgs.OutPatientID ?? "",
                                    ServiceProID = this.SelectedInputItem?.ServiceProID ?? "",
                                    GroupId = this.SelectedInputItem?.GroupID ?? "",
                                    Notes = "AuthNo:" + Data.ScreenArgs.AuthNumber + Environment.NewLine + $"Fax Cover Letter has been sent to {Data.ShippingDetails.PharmacyName}",
                                    StatusDesc = "Prescription",
                                    DocTypeDesc = this.RequestBase?.Type ?? "Authorization",
                                    LetterType = "",
                                    FunnctionalArea = "",
                                    LetterTypeId = this.RequestBase?.LetterTypeId ?? "",
                                    TaskID = this.RequestBase?.AAPTaskid ?? ""
                                };
                                this.InsertFaxLogFileResults = _model1.InsertFaxLogFile(objfaxlog);


                                if ((this.InsertFaxLogFileResults?.Text ?? "") != "")
                                {
                                    string TifFolder = string.Empty;
                                    string[] tifpath = InsertFaxLogFileResults.Text.Split('\\');
                                    if (tifpath[1].Length > 0)
                                    {
                                        TifFolder = "\\\\" + tifpath[1] + "\\" + tifpath[2] + "\\" + tifpath[3] + "\\" + tifpath[4];
                                    }
                                    else if (tifpath[2].Length > 0)
                                    {
                                        TifFolder = "\\\\" + tifpath[2] + "\\" + tifpath[3] + "\\" + tifpath[4];
                                    }
                                    else if (tifpath[3].Length > 0)
                                    {
                                        TifFolder = "\\\\" + tifpath[3] + "\\" + tifpath[4];
                                    }
                                    for (int k = 5; k <= tifpath.Length - 1; k++)
                                    {
                                        TifFolder = TifFolder + "\\" + tifpath[k];
                                        if (!(System.IO.Directory.Exists(TifFolder)))
                                        {
                                            System.IO.Directory.CreateDirectory(TifFolder);
                                        }
                                    }


                                    Thread.Sleep(300);
                                    string extension = Path.GetExtension(InsertFaxLogFileResults.Additional);

                                    if (extension == ".pdf")
                                    {
                                        deletepath = obj.Text ?? "";
                                        if (obj.Text.EndsWith(".doc")) { obj.Text = obj.Text.Replace(".doc", ".pdf"); }
                                        else if (obj.Text.EndsWith(".docx")) { obj.Text = obj.Text.Replace(".docx", ".pdf"); }
                                    }

                                    string targetpath = obj.Text ?? "";
                                    FileInfo fi = new FileInfo(obj.Text);
                                    fi.CopyTo(this.InsertFaxLogFileResults.Additional);
                                    fi = null;
                                }

                                SendFax(this.InsertFaxLogFileResults.Additional, this.InsertFaxLogFileResults.ID, RequestBase.Mode, FaxTriggerMode);
                            }
                            catch { Exception ex; }

                        }), System.Windows.Threading.DispatcherPriority.Send);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An exception captured " + ex.Message);
                    }
                    finally
                    {
                        try
                        {
                            bool exists = System.IO.File.Exists(deletepath);
                            if (exists)
                            {
                                System.IO.File.Delete(deletepath);
                            }

                            bool exists1 = System.IO.File.Exists(obj.Text);
                            if (exists1)
                            {
                                System.IO.File.Delete(obj.Text ?? "");
                            }
                            if (System.IO.File.Exists(res ?? ""))
                            {
                                System.IO.File.Delete(res ?? "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An exception captured " + ex.Message);
                        }
                    }
                })).Start();

                this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Information, $"Fax Cover Letter has been sent to {Data.ShippingDetails.PharmacyName}.");
            }
        }
        protected override bool OnValidateInputs()
        {

            FormValidation();
            try
            {
                if (Validate == true && Data.ShippingDetails.Status.Text == "Complete")
                {
                    foreach (var Drug in Data.DrugsList)
                    {
                        if (Drug.DrugName == "MENOPUR INJ 75 IU")
                        {
                            var WincardValidation = this.Model.GetWincardValidation(Data?.ScreenArgs?.MemberID ?? "", Data?.ScreenArgs?.Contract ?? "", Drug.DrugName ?? "", Drug.PharmacyLogin ?? "",Data?.ScreenArgs?.TaskID ?? "", "Audit");

                            if (WincardValidation?.ID == "N")
                            {
                                CreateWincard(Drug);
                                //Thread emailThread = new Thread(() => CreateWincard(Drug));
                                //emailThread.Start();                                
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
            return Validate;
        }
        public bool Validate { get; set; }
        private RequestBase _requestbase = new RequestBase();
        public RequestBase RequestBase
        {
            get { return _requestbase; }
            set { _requestbase = value; }
        }

        private void FormValidation()
        {
            Validate = true;
            this.IsButtonEnabled = true;
            try
            {
                //if (this.Data.NextTask.Attachments == null)
                //{
                //    this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Please attach the neccessary document.");
                //    Validate = false; return;
                //}

                if (this.Data.NextTask.Task.SendToQueue.Text == null)
                {
                    this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Please select Send To Pharmacy."); Validate = false; return;
                }
                if (this.Data.NextTask.Attachments != null)
                {
                    if (this.Data.NextTask.Attachments.Count == 0)
                    {

                        if (Utilities.Validations.ManagedContractsList(Data?.ScreenArgs?.Contract))
                        {
                            this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Please attach the neccessary documents."); Validate = false; return;
                        }
                        //else if (PREMIER.Contains(Data?.ScreenArgs?.Contract))                        
                        ObjectBase result1 = new ObjectBase();
                        result1 = new PremierAuthModel().GetATSContractCategories(this.Data.ScreenArgs.Contract ?? "", this.Data.AuthHeaderDetails.MemberId ?? "", this.Data.ScreenArgs.UserName ?? "");
                        if (result1.ID == "Y")
                        {
                            this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Please attach the neccessary documents."); Validate = false; return;
                        }
                        else if (Data?.NextTask?.Task?.SendToQueue.Text == "Freedom Fertility Pharmacy" && (Data?.ShippingDetails?.Status?.Text ?? "") == "Complete")
                        {
                            this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, "Please attach the neccessary documents."); Validate = false; return;
                        }
                    }

                }
                if (this.Data?.ShippingDetails?.Status?.Text != "Incomplete")
                {
                    if (!string.IsNullOrEmpty(Data?.ShippingDetails?.PBMError))
                    {
                        if (this.Data?.ShippingDetails?.PBMError != "No error messages to display")
                        {
                            this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, Data.ShippingDetails.PBMError + " " + ".Please contact IT"); Validate = false; return;
                        }
                    }
                }
                var WeekDay = this._Model.GetWeekDay(this.Data.ScreenArgs.Contract, this.Data.AuthHeaderDetails.MemberId, Data?.ShippingDetails?.CPTCode, this.Data.ScreenArgs.TaskName);
                if ((Data?.ShippingDetails?.Status?.Text ?? "") == "Complete" && (this.Data.ScreenArgs.TaskName == "Audit Approved Prescription") && this.Data.AuthHeaderDetails.Contract == "Boeing-BCBSIL")
                {
                    if (WeekDay == "Y")
                    {
                        return;
                    }
                    else
                    {
                        int result = this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Alert, WeekDay);
                        Validate = true; return;
                    }
                }

                var result4 = this.managedRxModel.GetHmorxpopup(Data.AuthHeaderDetails.MemberId ?? "", Data.ScreenArgs.Contract ?? "", Data.ScreenArgs.TaskName ?? "", Data?.SelectedTaskStatus ?? "", this.Data.ShippingDetails.Status.Text ?? "");
                if (result4?.ID == "Y")
                {

                    int res = this.IUIWindow.MsgWindow.Show(WinUIDTO.ATSMsgType.Confirmation, result4.Text, new string[] { result4.Additional1, result4.Additional2 });
                    if (res == 0)
                    {
                        Validate = true; return;
                    }
                    else if (res == 1)
                    {
                        Validate = false; return;
                    }

                }
                else { }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception captured " + ex.Message);
            }
        }
        protected override void OnPostInit()
        {
            try
            {
                this.Alerts = this.Model.GetAlerts(this.Data.ScreenArgs.Contract, this.Data.ScreenArgs.TaskName, this.Data.ScreenArgs.MemberID, "Task");
                if (this.Alerts.AlertList.Count > 0) { IsVisibleAlerts = true; } else { IsVisibleAlerts = false; }
                if (Data.ShippingDetails != null)
                {
                    this.Data.ShippingDetails.PropertyChanged += ShippingDetailsPropertyChanged;
                }
                this.Data.ShippingDetails.Status = new ObjectBase() { ID = "Incomplete", Text = "Incomplete" };
                //this.WorkQueueList = Data.WorkQueues;
                this.PharmacyList = Data.PharmacyList;
                Data.NextTask.ShceduledDateHeader = "Date Meds Needed";
                foreach (var Drug in Data.DrugsList)
                {
                    if (Drug.DrugName == "MENOPUR INJ 75 IU")
                    {
                        var WincardValidation = this.Model.GetWincardValidation(Data?.ScreenArgs?.MemberID ?? "",Data?.ScreenArgs?.Contract ?? "", Drug.DrugName ?? "", Drug.PharmacyLogin ?? "", Data?.ScreenArgs?.TaskID ?? "", "Audit");

                        if (WincardValidation?.ID == "N")
                        {
                            IsBusy = true;
                        }
                        break;
                    }
                }
                //this.WorkQueueList = this.TaskToList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception captured " + ex.Message);
            }
        }

        private void ShippingDetailsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (string.Compare(e.PropertyName, nameof(this.Data.ShippingDetails.Status), true) == 0)
                    {
                        if (Data.ShippingDetails.Status.Text == "Incomplete")
                        {
                            Data.NextTask.Task.TaskName = "Audit Approved Prescription";
                            //this.WorkQueueList = Data.TaskToList;
                            this.WorkQueueList = base.Model.GetTaskTo();
                            //Data.NextTask.Task.SendToQueue = new ObjectBase() { Text = "PBMAuditQue", ID = "PBMAuditQue" };
                            Data.NextTask.Task = this.Model.GetNextTask(new NextTaskParams()
                            {
                                Contract = Data.AuthHeaderDetails.Contract ?? "",
                                UserRole = Utilities.Common.Role,
                                TaskName = Data?.ScreenArgs?.TaskName ?? "Audit Approved Prescription",
                                TaskStatus = "Incomplete",
                                ContactFor = Data.ScreenArgs.ContactFor ?? "",
                                //AuthStatus = getAuthStatus ?? ""
                                //string.Compare("WINFertility DTC", Data?.AuthHeaderDetails?.Contract ?? "", true) == 0 ? "Behavioral Consult" : ""
                            });

                            Data.NextTask.Task.Priority = Data?.ShippingDetails?.Priority ?? "Medium";
                        }
                        else if (Data.ShippingDetails.Status.Text == "Complete")
                        {
                            Data.NextTask.Task.TaskName = "Process Prescription";
                            Data.WorkQueues = null;
                            this.WorkQueueList = this.PharmacyList;
                            //Data.WorkQueues = this.PharmacyList;
                            Data.NextTask.Task.SendToQueue = new ObjectBase() { ID = Data?.ShippingDetails?.PharmacyLogin ?? "", Text = Data?.ShippingDetails?.PharmacyName ?? "" };
                            Data.NextTask.Task.Priority = Data.NextTask.Task.Priority;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception captured " + ex.Message);
            }
        }
        public void CreateWincard(WinManagedDrugs Drug)
        {
            try
            {
                string RXCostshareValidateFlg = this.Model.LoadgetRXCostshareValidate(Data?.ScreenArgs?.Contract ?? "", Data?.ScreenArgs?.MemberID ?? "");
                decimal CostShare = 0;
                decimal amount = 0;
                decimal rxcost = 0;
                try
                {
                    rxcost = (decimal.TryParse(Drug.AWP, out decimal awp) && decimal.TryParse(Drug.ApprQty, out decimal apprQty)) ? awp * apprQty : 0;
                }
                catch { }

                {
                    if (Data.ScreenArgs.Contract == "Chewy-Aetna-Premier")
                    {
                        RequestBase args = new RequestBase()
                        {
                            Contract = Data?.ScreenArgs?.Contract ?? "",
                            MemberID = Data?.ScreenArgs?.MemberID ?? "",
                            AuthNo = Data?.ScreenArgs?.AuthNumber ?? "",
                        };
                        PremierRXModel Model11 = new PremierRXModel();
                        ObjectBase PlanandAvailityDetails = Model11.GetMemberCostSharePlanDetails(args);

                        string PlanName = PlanandAvailityDetails?.Additional ?? "";
                        if (!PlanName.Contains("PPO"))
                        {
                            CostShare = 0;
                        }
                        else
                        {
                            CostShare = 50;
                        }
                        amount = rxcost - CostShare;

                    }
                    else if (RXCostshareValidateFlg == "Y")
                    {
                        CostShare = this.Model.GetRXcostshare(Data?.ScreenArgs?.Contract ?? "", rxcost,
                           Data?.ScreenArgs?.MemberID ?? "", Data?.ScreenArgs?.AuthNumber ?? "", Data?.ScreenArgs?.TaskID ?? "");
                        amount = rxcost - CostShare;
                    }
                    else
                    {
                        amount = rxcost;
                    }
                    //amount = 99;
                    if(Data.ScreenArgs.MemberID.ToLower().Contains("test"))
                    {
                        amount = 1;
                    }
                    if (amount > 0)
                    {
                        var purchaseRequest = new WinCPSDTO.TeamPayPurchaseRequest()
                        {
                            Amount = amount,
                            Currency = "USD",
                            External_Requestor_Id = Guid.NewGuid().ToString(),
                            Vendor_Name = Data?.ShippingDetails?.PharmacyName ?? "",
                            Category = "Menopur Drug",
                            Description = "Menopur"
                        };
                        var result = this.Model.purchaseRequest(purchaseRequest);
                        if (result != null && result?.success == true)
                        {
                            PremierRXModel _model1 = new PremierRXModel();
                            _model1.SaveTeamPayRequestResponseDetails(new WinDTO.CPS.TeamPayRequestInput()
                            {
                                MemberId = this.Data.ScreenArgs.MemberID,
                                TaskId = Convert.ToInt32(this.Data.ScreenArgs.TaskID),
                                Contract = this.Data.ScreenArgs.Contract,
                                RequestedLocalAmount = purchaseRequest.Amount,
                                RequestedLocalCurrency = purchaseRequest.Currency,
                                RequestedCurrency = purchaseRequest.Currency,
                                RequestedUSDAmount = purchaseRequest.Amount,
                                RequestId = purchaseRequest.External_Requestor_Id,
                                Buffer = 0,
                                LastUser = Utilities.Common.UserName,
                                TeamPayWinCardURL = result.purchase_request_link,
                                USDEquivalentAmount = purchaseRequest.Amount,
                                ClaimId = "",
                                AuthNo = this.Data.ScreenArgs.FormattedAuthNumber,
                                CostShare = CostShare
                            });

                            try
                            {
                                Thread emailThread = new Thread(() => SendWincardEmail());
                                emailThread.Start();
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }
        private void SendWincardEmail()
        {
            try
            {
                Mail obj = new Mail();
                var RedirectLink = this.Model.GenerateAppRedirectLink(this.Data.ScreenArgs.MemberID ?? "", this.Data.ScreenArgs.Contract);
                obj = this.Model.GetWincardMailDetails(this.Data.ScreenArgs.MemberID ?? "", this.Data.ScreenArgs.Contract, RedirectLink ?? "");
                if (obj.Validation == "Y")
                {
                    //string[] emailto = (obj?.To).Split(',');
                    //string[] emailcc = (obj?.Cc)?.Split(',');
                    //string[] emailbcc = (obj?.Bcc).Split(',');
                    //obj.To = "saiteja@win-healthcare.com";
                    SendMailModel sendMailModel = new SendMailModel();
                    sendMailModel.SendMail1(new RequestBase()
                    {
                        UserID = obj.From ?? "",
                        Contract = this.Contract
                    },
                                               obj.From ?? "", obj?.To.Split(','), obj?.Cc.Split(','), obj?.Bcc.Split(','),
                                                obj.Subject, obj.Body,
                                         this.Contract, null, NEWWin_Logo);

                    //sendMailModel.SendMail(new RequestBase { UserID = obj?.From ?? "", Contract = this.Contract ?? "" }, obj?.From ?? "", emailto, emailcc, emailbcc, obj.Subject ?? "", obj.Body ?? "", "", "");
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    this.Model.SaveNotes(obj.Other ?? "", this.Data.ScreenArgs.MemberID ?? "", this.Data.ScreenArgs.Contract, Data?.ScreenArgs?.AuthNumber ?? "");
                }
            }
            catch { }
        }
        #endregion

    }
}