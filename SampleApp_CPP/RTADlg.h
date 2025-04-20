
#pragma once

#include <vector>
#include <algorithm>  
#include <string>
#include <afxcmn.h>  
#include "ScannerCommands.h"
#include <List>
#include "RtaList.h"
#include "RsmList.h"

class CRTADlg : public CDialog
{
    DECLARE_DYNAMIC(CRTADlg)

public:
    CRTADlg(CWnd* pParent = NULL);
    virtual ~CRTADlg();
    //Dialog specific functions
    virtual BOOL OnInitDialog();
    afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

    //SSDK- RTA Event Log Implementation
    void UpdateRtaEvent(BSTR rtaData);

    enum
    {
        IDD = IDD_RTA
    };

    void SetScannerID(wstring* pScannerID);
    void SetAsync(int* pAsync);
    afx_msg void OnCheckedSuspendReportingAlerts();
    afx_msg void OnListConfigurationSettings();
    afx_msg void OnListEventLog();
    afx_msg void OnGetSupportedRTAEvents();
    afx_msg void OnGetRegisteredRTAEvents();
    afx_msg void OnRegisterRTAEvents();
    afx_msg void OnSetRTAEventStatus();
    afx_msg void OnGetRTAEventStatus();
    afx_msg void OnClearAll();
    afx_msg void OnGetRTAState();
    afx_msg void OnClearEventLog();

    vector<vector<wstring>> GetSupportedRTA();
    void UIButtonsState(BOOL State);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    DECLARE_MESSAGE_MAP()
    DECLARE_TAB_WINDOW(RTA)
    BOOL RtaSuspend;
    CString RtaState;

private:
    wstring m_ScannerID;
    int m_Async;
    CScannerCommands* m_pScannerCommands;
    CBrush  m_brush;

    // Column and event-related constants
    const wstring OnLimitColumn = L"OnLimit";
    const wstring OffLimitColumn = L"OffLimit";
    const wstring EventColumn = L"Event";
    const wstring StatColumn = L"Stat";
    const wstring RegisteredColumn = L"Registered";

    // RTA-related constants
    const wstring GiftedBatteryPercentageAttribute = L"30012";
    const wstring ScannerOutOfCradleAttribute = L"38004";
    const wstring ScannerIdleAttribute = L"38001";
    const wstring RtaLimitNotSet = L"Not set";
    const wstring RtaLimitNotSupported = L"Not applicable";

    // Range limits
    const int RtaRangeMinLimit = 5;
    const int RtaRangeMaxLimitGiftedBatt = 95;
    const int RtaRangeMaxLimitScannerOutOfCradle = 600;

    // Stat validation codes
    const int RtaStatOverMax = 7;
    const int RtaStatBelowMin = 9;

    // Error messages
    const CString RegisterRtaErrorTitle = L"ERROR - Register RTA Events";
    const wstring RtaInvalidOnLimitMessage = L"Invalid values set for On-Limit";
    const wstring RtaInvalidOffLimitMessage = L"Invalid values set for Off-Limit";

    // Common strings
    const wstring forText = L" for ";
    const wstring EventPrefix = L"Event: ";
    const wstring StatPrefix = L"Stat: ";

    struct RtaEventDetails
    {
        wstring Event;
        wstring Stat;
        wstring OnLimit;
        wstring OffLimit;
    };

    // Additional controls or functions need for the RTA tab
    CRtaListCtrl RtaEventLogGridControl;
    CRtaListCtrl RtaEventsGrid;
    void InitRTAEventsLogAttribList();
    void InitRTAGetEventsAttribList();
    void InitRTAGetSupportedList();
    void SeparateSupportedandRegistered(BOOL registeredOnly);
    void UpdateRTAGrid(vector<vector<wstring>> eventDetailsList);
    vector<vector<wstring>> GetRTAEventsStatus();
    BOOL GetRTASuspendStatus();

    CButton GetSupportedRTAEvents;
    CButton GetRegisteredRTAEvents;
    CButton RegisterRTAEvents;
    CButton SetRTAEventStatus;
    CButton GetRTAEventStatus;
    CButton ClearAll;
    CButton GetRTAState;
    CButton ClearEvents;
    CButton RTASuspendCheck;
};
