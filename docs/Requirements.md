# Functional Requirements - GestioneCondominio

## 1. Administration Firm Management

### 1.1 Registration and Firm Profile
- Register a new administration firm
- Manage firm details (business name, VAT number, address, contacts, certified email / PEC)
- Manage users belonging to the firm (administrators, collaborators) with roles and permissions
- Invite new users to the firm via email

### 1.2 Condominium Sharing and Transfer
- Transfer condominium management from one firm to another (change of administrator)
- Share a condominium with other firms (operational delegation)
- The owner firm always retains management responsibility
- Delegate firms operate on behalf of the owner firm with configurable permissions
- Full history of transfers and sharing events

## 2. Condominium Management

### 2.1 Condominium Registry
- Create and edit a condominium (name, address, tax code, IBAN)
- Manage property units (apartments, garages, cellars, shops, offices)
- Associate property owners to property units
- Manage ownership periods (ownership history per unit)

### 2.2 Apportionment Tables (Tabelle Millesimali)
- Manage multiple apportionment tables per condominium (ownership, stairs, elevator, heating, etc.)
- Manual entry of apportionment values per property unit
- Import apportionment tables from Excel files
- Import apportionment tables from PDF files (with parsing)
- Automatic validation (sum of values = 1000)

### 2.3 Staircases and Building Sections
- Manage multiple staircases within a condominium
- Associate property units to staircases
- Manage separate building sections

## 3. Property Owner Management

### 3.1 Owner Registry
- Register and manage owner personal data (name, surname, tax code, contacts, residence)
- A property owner may own units in different condominiums, even managed by different firms
- Unified view for the owner across all their properties

### 3.2 Owner Portal (Read-Only)
- View owned property units and their apportionment values
- View personal accounting status (installments, payments, balance)
- View condominium documents (regulations, minutes, financial statements)
- View received communications
- View budgets and financial statements

## 4. Accounting

### 4.1 Chart of Accounts
- Customizable chart of accounts per condominium
- Predefined expense categories with customization options

### 4.2 Accounting Entries
- Record income (condominium installments, extraordinary contributions, other receipts)
- Record expenses (supplier invoices, utilities, miscellaneous costs)
- Associate expenses to chart of accounts categories
- Associate expenses to apportionment tables for cost distribution

### 4.3 Expense Distribution
- Automatic expense distribution based on apportionment tables
- Distribution by equal shares
- Distribution by consumption (e.g., water, heating)
- Custom distribution
- Preview distribution before confirmation

### 4.4 Installments and Payments
- Generate installment plans (ordinary and extraordinary)
- Record owner payments
- Manage payment reminders for overdue installments
- Owner account statement (ledger extract)

### 4.5 Financial Statements
- Generate annual final statement (consuntivo)
- Generate annual budget (preventivo)
- Compare final statement vs budget
- Import existing final statements from Excel files
- Import existing final statements from PDF files (with parsing)
- Import existing budgets from Excel files
- Import existing budgets from PDF files (with parsing)
- Export financial statements to PDF and Excel
- Print financial statements

## 5. Assembly Management

### 5.1 Convocation
- Create an assembly with date, time, location (or link for online meetings)
- Define the agenda
- Generate and send convocation notices to property owners
- Manage first and second convocation

### 5.2 Proceedings
- Record attendance and proxies
- Automatic quorum calculation (by head count and apportionment values)
- Manage voting for each agenda item
- Automatic majority calculation

### 5.3 Minutes
- Generate assembly minutes
- Record resolutions
- Archive minutes in the document management system

## 6. Supplier and Maintenance Management

### 6.1 Supplier Registry
- Manage supplier data (business name, VAT number, contacts, IBAN, service categories)
- Supplier intervention history

### 6.2 Contracts
- Manage supplier contracts (elevator maintenance, staircase cleaning, gardening, etc.)
- Contract expiry calendar with renewal alerts
- Archive contracts in the document management system

### 6.3 Interventions and Maintenance
- Record ordinary and extraordinary maintenance interventions
- Issue reports / fault tickets from property owners
- Assign intervention to a supplier
- Track intervention status (reported, assigned, in progress, completed)
- Intervention history per condominium

## 7. Communications

### 7.1 Internal Communications
- Send communications from administrator to owners (individual, group, all)
- Communication board per condominium
- In-app notifications for new communications

### 7.2 Notifications
- Email notifications for important events (new communication, assembly convocation, installment due, payment reminder)
- Notification preference settings per user

## 8. Document Management System

### 8.1 Document Archive
- Upload and archive documents with categorization:
  - Invoices and expense reports
  - Apportionment tables
  - Budgets (preventivi)
  - Final statements (consuntivi)
  - Communications
  - Condominium regulations
  - Contracts
  - Assembly minutes
- Associate documents to the relevant condominium
- Associate documents to the management year
- Search documents by category, date, condominium

### 8.2 Viewing and Download
- Document preview in browser
- Single document download
- Bulk download (ZIP)

## 9. User Management and Security

### 9.1 Authentication
- Login with local credentials (email + password)
- Login with external providers:
  - Microsoft Account
  - Microsoft Entra ID (Azure AD)
  - Google
  - Google Workspace
  - Apple
  - Facebook
  - X (Twitter)
- New user registration
- Password reset
- Two-factor authentication (2FA)

### 9.2 Roles and Permissions
- **Super Admin**: platform management
- **Firm Admin**: full management of the firm and associated condominiums
- **Firm Operator**: operational access to assigned condominiums with configurable permissions
- **Property Owner**: read-only access to own relevant data
- Granular configurable permissions for delegate firms

## 10. Dashboard and Reporting

### 10.1 Administrator Dashboard
- Overview of managed condominiums
- Collection and delinquency status
- Upcoming deadlines (contracts, assemblies, obligations)
- Ongoing interventions

### 10.2 Owner Dashboard
- Summary of owned property units
- Payment status and balance
- Upcoming installment due dates
- Recent communications

### 10.3 Reports
- Delinquency report per condominium
- Expense report by category
- Overall accounting status report
- Export reports to PDF and Excel
