namespace WallsShop.DTO;

public class AddOfferDto
{
    public string Name { get; set; }
    
    public string ArabicName { get; set; }
    
    public string Description { get; set; }
    
    public string ArabicDescription { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    public string CateogryValue { get; set; }
}



/* <Project Sdk = "Microsoft.NET.Sdk.Web" >


	< PropertyGroup >

		< TargetFramework > net9.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include = "Azure.Storage.Blobs" Version="12.27.0-beta.1" />
        <PackageReference Include = "MailKit" Version="4.14.1" />
        <PackageReference Include = "Microsoft.AspNetCore.Authentication" Version="2.3.9" />
        <PackageReference Include = "Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
        <PackageReference Include = "Microsoft.AspNetCore.Identity" Version="2.3.1" />
        <PackageReference Include = "Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.0" />
        <PackageReference Include = "Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
        <PackageReference Include = "Microsoft.AspNetCore.SignalR.Common" Version="10.0.1" />
        <PackageReference Include = "Microsoft.EntityFrameworkCore" Version="9.0.0" />
        <PackageReference Include = "Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        <PackageReference Include = "Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
        <PackageReference Include = "Microsoft.EntityFrameworkCore.Tools" Version="10.0.1">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        <PackageReference Include = "Microsoft.IdentityModel.Tokens" Version="8.15.0" />
        <PackageReference Include = "System.IdentityModel.Tokens.Jwt" Version="8.15.0" />
    </ItemGroup>

    <ItemGroup>
      <Folder Include = "wwwroot\" />
	</ ItemGroup >

</ Project >*/



