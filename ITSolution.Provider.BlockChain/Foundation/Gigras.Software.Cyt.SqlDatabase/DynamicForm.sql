
/****** Object:  Table [dbo].[IT_DynamicCtrl]    Script Date: 17-12-2024 15:55:17 ******/
CREATE TABLE [dbo].[IT_Admin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[UserName] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](255) NULL,
	[Avatar] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NULL,
	[LastLogin] [datetime] NULL,
	[IsActive] [bit] NULL,
	[Token] [nvarchar](500) NULL,
	[TokenExpiry] [datetime] NULL,
 CONSTRAINT [PK_IT_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[IT_Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[UserName] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](255) NULL,
	[Avatar] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[LastLogin] [datetime] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_IT_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IT_Master_Countries')
BEGIN
    CREATE TABLE dbo.IT_Master_Countries (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CountryName VARCHAR(255) NOT NULL,
 		IsoName VARCHAR(255) NULL,
		IsoCode INT NULL,
        CreatedBy VARCHAR(255),
        CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
        ModifiedBy VARCHAR(255),
        ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
        IsActive BIT DEFAULT 1,
        IsDelete BIT DEFAULT 0
    );
END;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IT_Master_States')
BEGIN
CREATE TABLE dbo.IT_Master_States (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StateName VARCHAR(255) NOT NULL,
    CountryId INT NOT NULL,
    CreatedBy VARCHAR(255),
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedBy VARCHAR(255),
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BIT DEFAULT 1,
    IsDelete BIT DEFAULT 0,
    FOREIGN KEY (CountryId) REFERENCES IT_Master_Countries(Id)
);
END;

-- Table: IT_Cities
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IT_Master_Cities')
BEGIN
CREATE TABLE dbo.IT_Master_Cities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CityName VARCHAR(255) NOT NULL,
    StateId INT NOT NULL,
    CountryId INT NOT NULL,
    CreatedBy VARCHAR(255),
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedBy VARCHAR(255),
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BIT DEFAULT 1,
    IsDelete BIT DEFAULT 0,
    FOREIGN KEY (StateId) REFERENCES IT_Master_States(Id)
);
END;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IT_Master_Lookup')
BEGIN
    CREATE TABLE dbo.IT_Master_Lookup (
        Id INT PRIMARY KEY IDENTITY(1,1),         -- Unique identifier for each record
        Category VARCHAR(50) NOT NULL,             -- Category for the type of lookup (e.g., 'Currency', 'Unit', 'PaymentTerm')
        Code VARCHAR(50) NOT NULL,                 -- Code for the lookup value (e.g., 'USD', 'kg', 'NET30')
        Description VARCHAR(255) NOT NULL,         -- Full description of the lookup value (e.g., 'US Dollar', 'Kilogram', 'Net 30 Days')
        CreatedBy VARCHAR(255),                    -- User who created the record
        CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP, -- Timestamp when the record was created
        ModifiedBy VARCHAR(255),                   -- User who last modified the record
        ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP, -- Timestamp when the record was last modified
        IsActive BIT DEFAULT 1,                    -- Flag to indicate if the entry is active
        IsDelete BIT DEFAULT 0                    -- Flag to indicate if the entry is marked for deletion (soft delete)
   );
END;

CREATE TABLE [dbo].[IT_DynamicCtrl](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CtrlType] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicCtrl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFieldOptions]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFieldOptions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OptionName] [nvarchar](255) NULL,
	[IsDynamic] [bit] NOT NULL,
	[SourceTable] [nvarchar](255) NULL,
	[TextValueField] [nvarchar](500) NULL,
	[Condition] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFieldOptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFieldOptionValues]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFieldOptionValues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OptionId] [int] NULL,
	[OptionLabel] [nvarchar](255) NULL,
	[OptionValue] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFieldOptionValues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFieldTypes]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFieldTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CtrlType] [nvarchar](50) NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[FieldDescription] [nvarchar](255) NULL,
	[DefaultValue] [nvarchar](255) NULL,
	[FieldTypeOptionId] [int] NULL,
	[HasOptions] [bit] NOT NULL,
	[RequiresScript] [bit] NOT NULL,
	[MinValue] [int] NULL,
	[MaxValue] [int] NULL,
	[MaxLength] [int] NULL,
	[Steps] [decimal] NULL,
	[BlockChainFieldId] [int] NULL,
	[ComparerFieldId] [int] NULL,
	[RangeStart] [int] NULL,
	[RangeEnd] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFieldTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFieldTypeValidaion]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFieldTypeValidaion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FieldValidationId] [int] NULL,
	[FieldTypeId] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFieldTypeValidaion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFieldValidations]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFieldValidations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ValidationName] [nvarchar](500) NOT NULL,
	[ValidationType] [nvarchar](50) NOT NULL,
	[ValidationValue] [nvarchar](255) NULL,
	[ErrorMessage] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFieldValidations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFormFields]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFormFields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SectionId] [int] NULL,
	[FieldTypeId] [int] NULL,
	[FieldOrder] [int] NOT NULL,
	[CssClass] [nvarchar](max) NULL,
	[JavaScript] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFormFields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicForms]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicForms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FormName] [nvarchar](255) NOT NULL,
	[FormDescription] [text] NULL,
	[CountryId] [int] NULL,
	[StateId] [int] NULL,
	[CityId] [int] NULL,
	[EntityName] [nvarchar](200) NULL,
	[NavigationGroup] [nvarchar](50) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicForms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IT_DynamicFormsSection]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicFormsSection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FormId] [int] NULL,
	[SectionName] [nvarchar](255) NOT NULL,
	[SectionDescription] [text] NULL,
	[SortOrder] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicFormsSection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[IT_DynamicValidation]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicValidation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ValidationType] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_IT_DynamicValidation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[IT_DynamicUserData]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicUserData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FormId] [int] NULL,
	[UserData] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[OldUserId] [int] NULL,
 CONSTRAINT [PK_IT_DynamicUserData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[IT_DynamicUserData] ADD  CONSTRAINT [DF_IT_DynamicUserData_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[IT_DynamicUserData] ADD  CONSTRAINT [DF_IT_DynamicUserData_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
/****** Object:  Table [dbo].[IT_DynamicBlockChainField]    Script Date: 17-12-2024 15:55:17 ******/

CREATE TABLE [dbo].[IT_DynamicBlockChainField](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BlockChainField] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_IT_DynamicBlockChainField] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO