#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "RTADlg.h"
#include "afxdialogex.h"
#include <chrono>
#include <ctime>
#include <sstream>
#include <iomanip>
#include "QuickXmlParser.h"
#include "LogsDlg.h"
#include <vector>

// CRTADlg dialog

IMPLEMENT_DYNAMIC(CRTADlg, CDialogEx)

BOOL inRTAEventStatusView = FALSE;
BOOL inRegisterEventView = FALSE;

CRTADlg::CRTADlg(CWnd* pParent /*=nullptr*/)
	: CDialog(IDD_RTA, pParent)
{

}

CRTADlg::~CRTADlg()
{
}

void CRTADlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_RTAEventsLog, RtaEventLogGridControl);
	DDX_Control(pDX, IDC_LIST3, RtaEventsGrid);
	DDX_Check(pDX, IDC_CHECK1, RtaSuspend);
	DDX_Text(pDX, IDC_RTA_STATE, RtaState);

	DDX_Control(pDX, IDC_BUTTON1, GetSupportedRTAEvents);
	DDX_Control(pDX, IDC_BUTTON2, GetRegisteredRTAEvents);
	DDX_Control(pDX, IDC_BUTTON3, RegisterRTAEvents);
	DDX_Control(pDX, IDC_BUTTON4, SetRTAEventStatus);
	DDX_Control(pDX, IDC_BUTTON5, GetRTAEventStatus);
	DDX_Control(pDX, IDC_BUTTON6, ClearAll);
	DDX_Control(pDX, IDC_BUTTON7, GetRTAState);
	DDX_Control(pDX, IDC_BUTTON8, ClearEvents); 
	DDX_Control(pDX, IDC_CHECK1, RTASuspendCheck);
}


BEGIN_MESSAGE_MAP(CRTADlg, CDialog)
	ON_BN_CLICKED(IDC_CHECK1, &CRTADlg::OnCheckedSuspendReportingAlerts)
	ON_BN_CLICKED(IDC_BUTTON1, &CRTADlg::OnGetSupportedRTAEvents)
	ON_BN_CLICKED(IDC_BUTTON2, &CRTADlg::OnGetRegisteredRTAEvents)
	ON_BN_CLICKED(IDC_BUTTON3, &CRTADlg::OnRegisterRTAEvents)
	ON_BN_CLICKED(IDC_BUTTON4, &CRTADlg::OnSetRTAEventStatus)
	ON_BN_CLICKED(IDC_BUTTON5, &CRTADlg::OnGetRTAEventStatus)
	ON_BN_CLICKED(IDC_BUTTON6, &CRTADlg::OnClearAll)
	ON_BN_CLICKED(IDC_BUTTON7, &CRTADlg::OnGetRTAState)
	ON_BN_CLICKED(IDC_BUTTON8, &CRTADlg::OnClearEventLog)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()

BOOL CRTADlg::OnInitDialog() //Initializing the headers of the RTA Event Log grid
{
	CDialog::OnInitDialog();
	InitRTAEventsLogAttribList();
	RtaEventsGrid.ClearList();
	RtaSuspend = FALSE;

	return TRUE;
}

void CRTADlg::InitRTAEventsLogAttribList()
{
	CRtaListCtrl::tooltips = {
		{L"30012", L"9",L"Gifted Batt. Percentage",L"Value below min",L""},
		{L"30012", L"7",L"Gifted Batt. Percentage",L"Value above max",L""},
		{L"38001", L"7",L"RTA Scanner idle",L"Value above max",L""},
		{L"38003", L"13",L"RTA virtual tether alert",L"Alarm",L""},
		{L"38004", L"7",L"RTA Scanner out of cradle",L"Value above min",L""},
		{L"616", L"2",L"Config file",L"State any",L""}
	};

	//Setting headers for the RTA Event Log grid
	RtaEventLogGridControl.SetHeader(0,_T("Date Time"), 120, TRUE);
	RtaEventLogGridControl.SetHeader(1,_T("Model"), 140, TRUE);
	RtaEventLogGridControl.SetHeader(2,_T("Serial Number"), 110, TRUE);
	RtaEventLogGridControl.SetHeader(3,_T("Event"), 50, TRUE);
	RtaEventLogGridControl.SetHeader(4,_T("Stat"), 50, TRUE);
	RtaEventLogGridControl.SetHeader(5,_T("Data 1"), 50, TRUE);
	RtaEventLogGridControl.SetHeader(6,_T("Data 2"), 50, TRUE);
}

void CRTADlg::InitRTAGetEventsAttribList()
{
	inRegisterEventView = FALSE;
	inRTAEventStatusView = TRUE;
	RtaEventsGrid.ClearList();  // Clear the existing list
	CRtaListCtrl::tooltips = {
		{L"30012", L"9",L"Gifted Batt. Percentage",L"Value below min",L""},
		{L"30012", L"7",L"Gifted Batt. Percentage",L"Value above max",L""},
		{L"38001", L"7",L"RTA Scanner idle",L"Value above max",L""},
		{L"38003", L"13",L"RTA virtual tether alert",L"Alarm",L""},
		{L"38004", L"7",L"RTA Scanner out of cradle",L"Value above min",L""},
		{L"616", L"2",L"Config file",L"State any",L""}
	};

	//Setting headers for the RTA Events grid
	RtaEventsGrid.SetHeader(0, _T("No"), 50, TRUE);
	RtaEventsGrid.SetHeader(1, _T("Event"), 60, TRUE);
	RtaEventsGrid.SetHeader(2, _T("Stat"), 50, TRUE);
	RtaEventsGrid.SetHeader(3, _T("Scope"), 50, TRUE);
	RtaEventsGrid.SetHeader(4, _T("Registered"), 80, FALSE);
	RtaEventsGrid.SetHeader(5, _T("Reported"), 80, FALSE);
	RtaEventsGrid.SetHeader(6, _T("Initialized"), 80, TRUE);
	RtaEventsGrid.SetHeader(7, _T("Measuring"), 80, TRUE);
}

void CRTADlg::InitRTAGetSupportedList()
{
	inRegisterEventView = TRUE;
	inRTAEventStatusView = FALSE;
	RtaEventsGrid.ClearList();  // Clear the existing list
	CRtaListCtrl::tooltips = {
		{L"30012", L"9",L"Gifted Batt. Percentage",L"Value below min",L"A value between 5% to 95% can be set",L"A value between 5% to 95% can be set"},
		{L"30012", L"7",L"Gifted Batt. Percentage",L"Value above max",L"A value between 5% to 95% can be set",L"A value between 5% to 95% can be set"},
		{L"38001", L"7",L"RTA Scanner idle",L"Value above max",L"A value between 5 mins to 600 mins can be set"},
		{L"38003", L"13",L"RTA virtual tether alert",L"Alarm"},
		{L"38004", L"7",L"RTA Scanner out of cradle",L"Value above min",L"A value between 5 mins to 600 mins can be set"},
		{L"616", L"2",L"Config file",L"State any"}
	};

	//Setting headers for the Supported RTA Events grid
	RtaEventsGrid.SetHeader(0, _T("No"), 50, TRUE);
	RtaEventsGrid.SetHeader(1, _T("Registered"), 80, FALSE);
	RtaEventsGrid.SetHeader(2, _T("Event"), 50, TRUE);
	RtaEventsGrid.SetHeader(3, _T("Stat"), 50, TRUE);
	RtaEventsGrid.SetHeader(4, _T("On-Limit"), 80, FALSE);
	RtaEventsGrid.SetHeader(5, _T("Off-Limit"), 80, FALSE);
}

void CRTADlg::SetScannerID(wstring* pScannerID)
{
	m_ScannerID = *pScannerID;
}

void CRTADlg::SetAsync(int* pAsync)
{
}

HBRUSH CRTADlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if (!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));

	SetLabelBkg(pDC, pWnd, IDC_STATIC);
	return m_brush;
}

string getCurrentDateTime() { //Adding current date and time to the RTA Event Log
	auto now = chrono::system_clock::now();
	time_t now_time = chrono::system_clock::to_time_t(now);
	tm* now_tm = localtime(&now_time);

	ostringstream oss;
	oss << put_time(now_tm, "%Y-%m-%d %H:%M:%S");
	return oss.str();
}

void CRTADlg::UIButtonsState(BOOL State) 
{
	OnClearAll();
	RTASuspendCheck.EnableWindow(State);
	GetSupportedRTAEvents.EnableWindow(State);
	GetRegisteredRTAEvents.EnableWindow(State);
	RegisterRTAEvents.EnableWindow(State);
	SetRTAEventStatus.EnableWindow(State);
	GetRTAEventStatus.EnableWindow(State);
	ClearAll.EnableWindow(State);
	GetRTAState.EnableWindow(State);
	ClearEvents.EnableWindow(State);
}

void CRTADlg::UpdateRtaEvent(BSTR rtaData)
{
	try {
		int itemIndex = RtaEventLogGridControl.GetItemCount();

		// Convert date string to CString
		CString cstr(getCurrentDateTime().c_str());

		// Convert CString to LPCTSTR
		LPCTSTR time = cstr;

		// Insert the main item (this will usually correspond to the first column)
		int nIndex = RtaEventLogGridControl.InsertItem(itemIndex, time);

		CQuickXmlParser x(rtaData);
		CQuickXmlParser::TAGDATA tag[6] = { 0 };

		tag[0].Tag.Name = L"modelnumber";
		tag[1].Tag.Name = L"serialnumber";
		tag[2].Tag.Name = L"id";
		tag[3].Tag.Name = L"type";
		tag[4].Tag.Name = L"data-1";
		tag[5].Tag.Name = L"data-2";

		x.Configure(tag, 6);
		CQuickXmlParser::xptr p = 0;
		p = x.Parse(p);

		// Set the subitem texts (for the subsequent columns)
		RtaEventLogGridControl.SetItemText(nIndex, 1, x.Translate(tag[0].Value));  // Second column
		RtaEventLogGridControl.SetItemText(nIndex, 2, x.Translate(tag[1].Value));  // Third column
		RtaEventLogGridControl.SetItemText(nIndex, 3, x.Translate(tag[2].Value));  // Fourth column
		RtaEventLogGridControl.SetItemText(nIndex, 4, x.Translate(tag[3].Value));  // Fifth column
		RtaEventLogGridControl.SetItemText(nIndex, 5, x.Translate(tag[4].Value));  // Sixth column
		RtaEventLogGridControl.SetItemText(nIndex, 6, x.Translate(tag[5].Value));  // Seventh column

		x.ClearValues(tag);

		//OnGetRTAEventStatus(); //Update the RTAEventsGrid
	}
	catch (exception ex) {
		LOG(-1, "Update RTA Event Error: {}",ex);
	}

}

void CRTADlg::OnListConfigurationSettings()
{
	// TODO: Add your control notification handler code here
}

void CRTADlg::OnListEventLog()
{
	// TODO: Add your control notification handler code here
}

void CRTADlg::OnCheckedSuspendReportingAlerts()
{
	try {
		RtaSuspend = (RtaSuspend == FALSE ? TRUE : FALSE);
		long status = -1;
		CComBSTR outXml(L"");
		CString log;
		log.Format(L"RTA Suspend State : %s", RtaSuspend ? L"Enabled" : L"Disabled");
		wchar_t* logStr = log.GetBuffer();
		SC->cmdSuspendRTAEvent(m_ScannerID, &outXml, &status, RtaSuspend);
		if (status != 0) {
			GetTabManager().GetTabDlg<CLogsDlg>().DisplayResult(status, L"RTA_SUSPEND_STATE");
		}
		GetTabManager().GetTabDlg<CLogsDlg>().DisplayResult(0, logStr);
	}
	catch (exception ex) {
		LOG(-1, "RTA_SUSPEND_STATE_UPDATE_ERROR: {}", ex);
		throw ex;
	}
}

void CRTADlg::OnGetSupportedRTAEvents()
{
	try 
	{
		InitRTAGetSupportedList();
		SeparateSupportedandRegistered(FALSE);
	}
	catch (exception ex){
		throw ex;
	}
}

void CRTADlg::OnGetRegisteredRTAEvents()
{
	try 
	{
		InitRTAGetSupportedList();
		SeparateSupportedandRegistered(TRUE);
	}
	catch (exception ex) {
		throw ex;
	}
}

void CRTADlg::SeparateSupportedandRegistered(BOOL registeredOnly)
{

	// Retrieve registered RTA events to get their IDs
	vector<vector<wstring>> eventDetailsList = GetRTAEventsStatus();
	if (eventDetailsList.empty()) return;
	
	GetRTASuspendStatus();

	vector<vector<wstring>> supportedRTAEvents = GetSupportedRTA();
	vector<vector<wstring>> supportedRTAEventsGrid;

	if (!supportedRTAEvents.empty()) {
		for (const auto& supportedEvent : supportedRTAEvents) {
			bool matchFound = false;

			for (const auto& event : eventDetailsList) {
				if (event[0] == supportedEvent[0] && event[1] == supportedEvent[1] && event[3] == L"TRUE") {
					supportedRTAEventsGrid.push_back({ L"TRUE", supportedEvent[0], supportedEvent[1], supportedEvent[2], supportedEvent[3] });
					matchFound = true;
					break; // No need to check further once a match is found
				}
			}

			if (!matchFound && !registeredOnly) {
				supportedRTAEventsGrid.push_back({ L"FALSE", supportedEvent[0], supportedEvent[1], supportedEvent[2], supportedEvent[3] });
			}
		}
	}

	if (supportedRTAEventsGrid.empty()) {
		MessageBox(_T("RTA Events are not registered in the selected device."), _T("INFO - Get Registered RTA Events"), MB_OK | MB_ICONINFORMATION);
		return;
	}

	//Update the grid with data
	UpdateRTAGrid(supportedRTAEventsGrid);
}

void CRTADlg::OnRegisterRTAEvents()
{
	try {
		if (RtaEventsGrid.GetItemText(0, 0) != "" && inRegisterEventView == TRUE) {
			vector<vector<wstring>> rtaEventsRegister;
			long status = -1;
			CComBSTR outXml(L"");

			for (auto& event : CRtaListCtrl::gridData)
			{
				if (event[0] == L"TRUE") {

					RtaEventDetails details;
					details.Event = event[1];
					details.Stat = event[2];
					details.OnLimit = event[3];
					details.OffLimit = event[4];

					int onLimitValue = -1, offLimitValue = -1;
					// Validate On-Limit and Off-Limit values should not be empty
					if (details.OnLimit.empty() || details.OffLimit.empty())
					{
						AfxMessageBox(L"On-Limit or Off-Limit should not be empty", MB_OK | MB_ICONERROR);
						return;
					}

					// Validate user is registering by not setting (value = "Not set") On-Limit/Off-Limit values
					if ((details.OnLimit == RtaLimitNotSet || details.OffLimit == RtaLimitNotSet))
					{
						CString errorMessage;
						errorMessage.Format(L"On-Limit/Off-Limit should be set for Event: %s and Stat: %s",
							details.Event.c_str(),
							details.Stat.c_str());
						AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
						return;
					}

					// Validations for Gifted Battery Percentage range limits
					else if (details.Event == GiftedBatteryPercentageAttribute)
					{
						// Validate On-Limit value
						if ((details.OnLimit != RtaLimitNotSet) && (details.OnLimit != RtaLimitNotSupported))
						{
							int onLimitValue = 0;
							if (swscanf_s(details.OnLimit.c_str(), L"%d", &onLimitValue) == 1)
							{
								if (onLimitValue > RtaRangeMaxLimitGiftedBatt || onLimitValue < RtaRangeMinLimit)
								{
									CString errorMessage;
									errorMessage.Format(L"On-Limit value is out of range for Event: %s and Stat: %d",
										GiftedBatteryPercentageAttribute.c_str(),
										RtaStatOverMax);
									AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
									return;
								}
							}
							else
							{
								CString errorMessage;
								errorMessage.Format(L"Invalid On-Limit value for Event: %s and Stat: %d",
									GiftedBatteryPercentageAttribute.c_str(),
									RtaStatOverMax);
								AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
								return;
							}
						}

						// Validate Off-Limit value
						if ((details.OffLimit != RtaLimitNotSet) && (details.OffLimit != RtaLimitNotSupported))
						{
							int offLimitValue = 0;
							if (swscanf_s(details.OffLimit.c_str(), L"%d", &offLimitValue) == 1) // Use swscanf_s for safe conversion
							{
								if (offLimitValue > RtaRangeMaxLimitGiftedBatt || offLimitValue < RtaRangeMinLimit)
								{
									CString errorMessage;
									errorMessage.Format(L"Off-Limit value is out of range for Event: %s and Stat: %d",
										GiftedBatteryPercentageAttribute.c_str(),
										RtaStatOverMax);
									AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
									return;
								}
							}
							else
							{
								CString errorMessage;
								errorMessage.Format(L"Invalid Off-Limit value for Event: %s and Stat: %d",
									GiftedBatteryPercentageAttribute.c_str(),
									RtaStatOverMax);
								AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
								return;
							}
						}

						int currentStat = -1;
						if (swscanf_s(details.Stat.c_str(), L"%d", &currentStat) == 1)
						{
						swscanf_s(details.OnLimit.c_str(), L"%d", &onLimitValue);
						swscanf_s(details.OffLimit.c_str(), L"%d", &offLimitValue);

							if (currentStat == RtaStatOverMax) // Max value Stat
							{
								if ((offLimitValue > 0) && (onLimitValue > 0) && (offLimitValue >= onLimitValue))
								{
									CString errorMessage;
									errorMessage.Format(L"Off-Limit value should be less than On-Limit for Event: %s and Stat: %d",
										GiftedBatteryPercentageAttribute.c_str(),
										RtaStatOverMax);
									AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
									return;
								}
							}
							else if (currentStat == RtaStatBelowMin) // Min value Stat
							{
								if ((offLimitValue > 0) && (onLimitValue > 0) && (offLimitValue <= onLimitValue))
								{
									CString errorMessage;
									errorMessage.Format(L"Off-Limit value should be greater than On-Limit for Event: %s and Stat: %d",
										GiftedBatteryPercentageAttribute.c_str(),
										RtaStatBelowMin);
									AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
									return;
								}
							}
						}
					}

					// Validate Scanner Out Of Cradle and Scanner Idle attributes
					if ((details.Event == ScannerOutOfCradleAttribute) || (details.Event == ScannerIdleAttribute))
					{
						int onLimitValue = -1;
						int offLimitValue = -1;

						// Validate On-Limit value
						if ((details.OnLimit != RtaLimitNotSet) && (details.OnLimit != RtaLimitNotSupported))
						{
							if (swscanf_s(details.OnLimit.c_str(), L"%d", &onLimitValue) == 1) // Use swscanf_s for safe conversion
							{
								if (onLimitValue > RtaRangeMaxLimitScannerOutOfCradle || onLimitValue < RtaRangeMinLimit)
								{
									CString errorMessage;
									errorMessage.Format(L"On-Limit value is out of range for Event: %s and Stat: %s",
										details.Event.c_str(),
										details.Stat.c_str());
									AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
									return;
								}
							}
							else
							{
								CString errorMessage;
								errorMessage.Format(L"Invalid On-Limit value for Event: %s and Stat: %s",
									details.Event.c_str(),
									details.Stat.c_str());
								AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
								return;
							}
						}

						// Validate Off-Limit value
						if ((details.OffLimit != RtaLimitNotSet) && (details.OffLimit != RtaLimitNotSupported))
						{
							if (swscanf_s(details.OffLimit.c_str(), L"%d", &offLimitValue) == 1) // Use swscanf_s for safe conversion
							{
								if (offLimitValue > RtaRangeMaxLimitScannerOutOfCradle || offLimitValue < RtaRangeMinLimit)
								{
									CString errorMessage;
									errorMessage.Format(L"Off-Limit value is out of range for Event: %s and Stat: %s",
										details.Event.c_str(),
										details.Stat.c_str());
									AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
									return;
								}
							}
							else
							{
								CString errorMessage;
								errorMessage.Format(L"Invalid Off-Limit value for Event: %s and Stat: %s",
									details.Event.c_str(),
									details.Stat.c_str());
								AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
								return;
							}
						}

						// Validate if Off-Limit is greater than On-Limit
						if ((offLimitValue > 0) && (onLimitValue > 0) && (offLimitValue <= onLimitValue))
						{
							CString errorMessage;
							errorMessage.Format(L"Off-Limit value should be greater than On-Limit for Event: %s and Stat: %s",
								details.Event.c_str(),
								details.Stat.c_str());
							AfxMessageBox(errorMessage, MB_OK | MB_ICONERROR);
							return;
						}
					}

					rtaEventsRegister.push_back({ event[1], event[2], event[3], event[4]});
				}
			}
			if (!rtaEventsRegister.empty()) {
				SC->cmdRegisterRTAEvents(m_ScannerID, &status, rtaEventsRegister);
				LOG(status,"RTA_REGISTER_EVENTS")
			}
			else {
				AfxMessageBox(L"No valid events to register", MB_OK | MB_ICONERROR);
				return;
			}
		}
		 else {
			MessageBox(_T("No data available to register RTA Events. Retrieve Supported/Registered RTA events, select new registrations and then proceed with registering RTA events"), (RegisterRtaErrorTitle), MB_OK | MB_ICONERROR);
			return;
		}
	}
	catch (exception ex) {
		throw ex;
	}
}

void CRTADlg::OnSetRTAEventStatus() {
	// Check if the grid has data in the first cell
	if (RtaEventsGrid.GetItemText(0, 0) != "" && inRTAEventStatusView == TRUE) {
		long status = -1;
		CComBSTR outXml(L"");

		// Retrieve grid data and event details
		vector<vector<wstring>> gridData = CRtaListCtrl::gridData;
		vector<vector<wstring>> eventDetailsList = GetRTAEventsStatus();

		// Prepare a list of events that need to be updated
		vector<vector<wstring>> setRTAEventsList;
		vector<vector<wstring>> unregRTAEventsList;
		size_t eventCount = min(gridData.size(), eventDetailsList.size());

		for (size_t i = 0; i < eventCount; ++i) {
			const vector<wstring>& rtaEvent = gridData[i];
			const vector<wstring>& eventDetails = eventDetailsList[i];

			// Compare specific field (index 4) and prepare events for update
			if (rtaEvent[4] != eventDetails[4]) {
				vector<wstring> event = {
					rtaEvent[0], // Event ID
					rtaEvent[1], // Stat
					rtaEvent[4]  // Status to be updated
				};

				setRTAEventsList.push_back(event);
			}

			if (rtaEvent[3] != eventDetails[3] && rtaEvent[3] == L"FALSE") {
				vector<wstring> event = {
					rtaEvent[0], //Event ID
					rtaEvent[1] //Stat
				};

				unregRTAEventsList.push_back(event);
			}
		}

		// Send the updates to the system
		if (!setRTAEventsList.empty()) {
			SC->cmdSetRTAEventStatus(m_ScannerID, &outXml, &status, setRTAEventsList);
		}

		if (!unregRTAEventsList.empty()) {
			SC->cmdUnregisterRTAEvent(m_ScannerID, &outXml, &status, unregRTAEventsList);
		}

		LOG(status, "RTA_SET_EVENT_STATUS");
	}
	else {
		// Display an error message if no data is available
		MessageBox(_T("No data available to set RTA Alert Status. Get RTA event status, select new status updates and then proceed with Setting RTA event status."),_T("ERROR - Set RTA Event Status"), MB_OK | MB_ICONERROR);
		LOG(-1, "RTA get event status error: No data available. Please press 'Get RTA Event Status'.");
	}
}

void CRTADlg::OnGetRTAEventStatus()
{
	try {
		InitRTAGetEventsAttribList();  // Initialize the attributes list

		vector<vector<wstring>> eventDetailsList = GetRTAEventsStatus(); // Retrieve event status and XML
		if (eventDetailsList.empty()) return;

		GetRTASuspendStatus();
		
		UpdateRTAGrid(eventDetailsList); //Update the grid with data
	}
	catch (...) {
		
		LOG(-1, "RTA_GET_EVENT_STATUS"); // Catch any other exceptions
	}

}

void CRTADlg::OnClearAll()
{
	RtaEventsGrid.ClearList();  // Clear the existing list
	RtaSuspend = FALSE; //Clear the suspend checkbox state
	UpdateData(FALSE); //Updating the UI
}

void CRTADlg::OnGetRTAState()
{
	map<int, CString> RtaStates =
	{
		{0, L"RTA Suspended" },
		{ 1, L"RTA Awaiting Registration" },
		{ 2, L"RTA Awaiting Context Address" },
		{ 3, L"RTA Fully Operational" }
	};

	long status = -1;
	CComBSTR outXml(L"");

	try {
		// Get the RTA event status
		if (!SC->cmdGetRTAState(m_ScannerID, &outXml, &status)) {

			if (status != 0) {
				LOG(status, "RTA_GET_STATE");
				return;
			}
			// Display XML response
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);

			// Configure XML parser
			CQuickXmlParser x(outXml);
			CQuickXmlParser::TAGDATA tag[1] = { 0 };

			tag[0].Tag.Name = L"state";

			x.Configure(tag, 1);

			// Parse XML and collect event details
			CQuickXmlParser::xptr p = 0;
			while (true) {
				p = x.Parse(p);
				RtaState = RtaStates[_ttoi(x.Translate(tag[0].Value))];
				UpdateData(FALSE);
				if (p == 0) break;
				x.ClearValues(tag);
			}

			LOG(status, "RTA_GET_STATE");
		}
	}
	catch (exception ex) {
		throw ex;
	}
	
	return;
}

void CRTADlg::OnClearEventLog()
{
	try {
		RtaEventLogGridControl.DeleteAllItems(); //Clearing the RTA Event Log
	}
	catch (exception ex) {
		LOG(-1, "RTA_EVENT_LOG_CLEAR ERROR : {}", ex)
	}
	
}

vector<vector<wstring>> CRTADlg::GetSupportedRTA() { //Parse the outXML of Supported RTA Events
	long status = -1;
	CComBSTR outXml(L"");
	vector<vector<wstring>> supportedRTAEvents;

	// Retrieve supported RTA events
	if (!SC->cmdGetSupportedRTAEvents(m_ScannerID, &outXml, &status))
	{
		if (outXml.Length() == 0 || status != 0) {
			MessageBox(_T("Supported RTA events could not be retrieved from the selected device."), _T("ERROR - Get Supported RTA Events"), MB_OK | MB_ICONERROR);
			LOG(status, "RTA_GET_SUPPORTED_STATUS");
			return supportedRTAEvents;
		}

		// Update the log tab with the supported events XML
		GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);

		// Parse the XML and fill in the grid control with the data
		CQuickXmlParser x(outXml);
		CQuickXmlParser::TAGDATA tag[4] = { 0 };
		tag[0].Tag.Name = L"id";
		tag[1].Tag.Name = L"stat";
		tag[2].Tag.Name = L"onlimit";
		tag[3].Tag.Name = L"offlimit";

		x.Configure(tag, 4);
		CQuickXmlParser::xptr p = 0;

		while (1)
		{
			p = x.Parse(p);  // Parse the XML

			vector<wstring> event = {
				x.Translate(tag[0].Value),
				x.Translate(tag[1].Value),
				x.Translate(tag[2].Value),
				x.Translate(tag[3].Value),
			};

			supportedRTAEvents.push_back(event);

			if (p == 0) break;
			x.ClearValues(tag);  // Clear tag values for the next iteration
		}
	}
	LOG(status, "GET_SUPPORTED_RTA");
	return supportedRTAEvents;
}

vector<vector<wstring>> CRTADlg::GetRTAEventsStatus() { //Parse the outXML of RTA Events Status
	long status = -1;
	CComBSTR outXml(L"");
	vector<vector<wstring>> eventDetailsList;

	try {
		// Get the RTA event status
		if (!SC->cmdGetRTAEventStatus(m_ScannerID, &outXml, &status)) {
			// Handle empty response
			if (outXml.Length() == 0 || status != 0) {
				MessageBox(_T("RTA alert status could not be retrieved from the selected device."),_T("ERROR - Get RTA Event Status"), MB_OK | MB_ICONERROR);
				LOG(status, "RTA_GET_EVENT_STATUS");
				return eventDetailsList;
			}

			// Display XML response
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);

			// Configure XML parser
			CQuickXmlParser x(outXml);
			CQuickXmlParser::TAGDATA tag[7] = { 0 };

			tag[0].Tag.Name = L"id";
			tag[1].Tag.Name = L"stat";
			tag[2].Tag.Name = L"scope";
			tag[3].Tag.Name = L"registered";
			tag[4].Tag.Name = L"reported";
			tag[5].Tag.Name = L"initialized";
			tag[6].Tag.Name = L"measuring";

			x.Configure(tag, 7);

			// Parse XML and collect event details
			CQuickXmlParser::xptr p = 0;
			while (true) {
				p = x.Parse(p);

				vector<wstring> event = {
					x.Translate(tag[0].Value),
					x.Translate(tag[1].Value),
					x.Translate(tag[2].Value),
					x.Translate(tag[3].Value),
					x.Translate(tag[4].Value),
					x.Translate(tag[5].Value),
					x.Translate(tag[6].Value)
				};

				eventDetailsList.push_back(event);

				if (p == 0) break;
				x.ClearValues(tag);
			}

			// Log successful status retrieval
			LOG(status, "RTA_GET_EVENT_STATUS");
		}
	}
	catch (const exception& ex) {
		// Handle exceptions
		throw;
	}

	return eventDetailsList;
}

BOOL CRTADlg::GetRTASuspendStatus() //Parse the outXML of RTA State
{
	long status = -1;
	CComBSTR outXml(L"");
	CString rtaSuspendState;

	try {
		if (!SC->cmdGetRTAEventStatus(m_ScannerID, &outXml, &status)) {
			// Handle empty response
			if (outXml.Length() == 0) {
				AfxMessageBox(_T("Setting RTA State failed"));
				LOG(-1, "RTA get event status error - Empty response from CS");
				return FALSE;
			}

			// Display XML response
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);

			// Configure XML parser
			CQuickXmlParser x(outXml);
			CQuickXmlParser::TAGDATA tag[1] = { 0 };

			tag[0].Tag.Name = L"suspend";

			x.Configure(tag, 1);

			// Parse XML and collect event details
			CQuickXmlParser::xptr p = 0;
			while (true) {
				p = x.Parse(p);
				rtaSuspendState = x.Translate(tag[0].Value);
				if (p == 0) break;
				x.ClearValues(tag);
			}

			RtaSuspend = rtaSuspendState == L"TRUE" ? TRUE : FALSE;
			UpdateData(FALSE);
			// Log successful status retrieval
			//LOG(status, "GET_RTA_SUSPEND");
			return RtaSuspend;
		}
	}
	catch (exception ex){
		LOG(status, "GET_RTA_SUSPEND_ERROR");
		throw ex;
	}
}

void CRTADlg::UpdateRTAGrid(vector<vector<wstring>> eventDetailsList)
{
	// Update the static member of CRtaListCtrl
	CRtaListCtrl::gridData = eventDetailsList;

	if (!eventDetailsList.empty()) {
		// Insert and populate grid with event details
		for (size_t rowIndex = 0; rowIndex < eventDetailsList.size(); ++rowIndex) {
			const auto& event = eventDetailsList[rowIndex];

			int itemIndex = RtaEventsGrid.GetItemCount();
			int nIndex = RtaEventsGrid.InsertItem(itemIndex, L"");

			// Set item number for the first column
			RtaEventsGrid.SetItemText(nIndex, 0, to_wstring(nIndex + 1).c_str());

			// Set the subitem texts for the subsequent columns
			for (size_t colIndex = 0; colIndex < event.size(); ++colIndex) {
				RtaEventsGrid.SetItemText(nIndex, colIndex + 1, event[colIndex].c_str());
			}
		}
	}
}


