/****** Object:  Table [dbo].[auth_info]    Script Date: 1/11/2021 9:49:02 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[asp_membership_copy](
	[UserId] [nvarchar](max) NULL,
	[OldCipherText] [nvarchar](max) NULL,
	[OldClearText] [nvarchar](max) NULL,
	[Salt] [nvarchar](max) NULL,
	[NewCipherText] [nvarchar](max) NULL,
	[NewClearText] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

