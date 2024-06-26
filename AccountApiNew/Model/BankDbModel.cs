﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountApiNew.Model
{

    // accounts , beneficiary, transaction , acounttype, branchs , 
    public class FundTransferModel
    {
        public long sourceAccountId {  get; set; }
        public long destinationAccountId {  get; set; }
        public decimal amount {  get; set; }
    }
    public class CustomerDto
    {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? Pincode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class CustomerUpdateDto
    {

        [MaxLength(20)]
        public string? FirstName { get; set; }


        [MaxLength(20)]
        public string? LastName { get; set; }


        [MaxLength(50)]
        public string? AddressLine1 { get; set; }

        [MaxLength(50)]
        public string? AddressLine2 { get; set; }

        [MaxLength(50)]
        public string? AddressLine3 { get; set; }


        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits")]
        public int? Pincode { get; set; }


        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? PhoneNumber { get; set; }


        [EmailAddress]
        [MaxLength(50)]
        public string? EmailAddress { get; set; }


        public DateTime DateOfBirth { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }


        [MaxLength(50)]
        public string? Country { get; set; }
    }

    [Table("DocTypes")]
    [PrimaryKey("DocTypeId")]
    public class DocType
    {
        [Key]
   
        public int DocTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string? DocName { get; set; }

        public bool IsActive { get; set; }
    }

    [Table("Documents")]
    [PrimaryKey("DocId")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocId { get; set; }

        [Required]
        public byte[]? Documents { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

    

        [ForeignKey("DocType")]
        public int DocTypeId { get; set; }

       

        public bool IsActive { get; set; }
    }
    [Table("Roles")]
    [PrimaryKey("RoleId")]
    [Index("RoleName", IsUnique = true, Name = "IDX_Roles_Names")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(20)]
        public string? RoleName { get; set; }
    }

    [Table("Customers")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(20, ErrorMessage = "First name cannot exceed 20 characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(20, ErrorMessage = "Last name cannot exceed 20 characters.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Address line 1 is required.")]
        [MaxLength(50, ErrorMessage = "Address line 1 cannot exceed 50 characters.")]
        public string? AddressLine1 { get; set; }

        [MaxLength(50, ErrorMessage = "Address line 2 cannot exceed 50 characters.")]
        public string? AddressLine2 { get; set; }

        [MaxLength(50, ErrorMessage = "Address line 3 cannot exceed 50 characters.")]
        public string? AddressLine3 { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
        public int Pincode { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(50, ErrorMessage = "Email address cannot exceed 50 characters.")]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string? Country { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
    
    }

    [Table("Users")]
    [PrimaryKey("UserId")]
    [Index("Username", IsUnique = true, Name = "IDX_User_Names")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]

        public string? Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Password { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

    
    }
    public class PasswordChangeModel
    {
        public string? Username { get; set; }
        public string? NewPassword { get; set; }
    }

    [Table("Accounts")]
        [PrimaryKey("AccountId")]
        public class Account
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountId { get; set; }
            
            [Required]
            public decimal Balance { get; set; }

            [Required]
            public int wd_Quota { get; set; }

            [Required]
            public int dp_Quota { get; set; }

            [Required]
            public bool isActive { get; set; }

            [Required]
            public int CustomerID { get; set; }

            [Required]
            public int TypeID { get; set; }

            [Required]
            [StringLength(11)]
            public string? BranchID { get; set; }

           [ForeignKey("CustomerID")]
           public Customer? Customer {  get; set; }

            [ForeignKey("BranchID")]
            public Branch? Branch { get; set; }

        [ForeignKey("TypeID")]
        public AccountType? AccountType { get; set; }


    }

    [Table("Branches")]
    [PrimaryKey("BranchID")]
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(11)]
        public string? BranchID { get; set; }

        [Required]
        [StringLength(50)]
        public string? BranchName { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }

    [Table("AccountTypes")]
    [PrimaryKey("TypeID")]
    public class AccountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeID { get; set; }

        [Required]
        [StringLength(20)]
        public string? TypeName { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
    [Table("Beneficiaries")]
    [PrimaryKey("BenefID")]
    public class Beneficiary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BenefID { get; set; }

        [Required]
        public string? BenefName { get; set; }

        [Required]
        public long BenefAccount { get; set; }

        [Required]
        public string? BenefIFSC { get; set; }


        [Required]
        public long AccountId { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

    }
    public class AccountInputModel
    {
        public decimal Balance { get; set; }
        public int CustomerID { get; set; }
        public int TypeID { get; set; }
        public string? BranchID { get; set; }
    }


    public class BeneficiaryInputModel
    {
        [Required]
        public string? BenefName { get; set; }

        [Required]
        public long BenefAccount { get; set; }

        [Required]
        public string? BenefIFSC { get; set; }

        [Required]
        public long AccountId { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
    [Table("Transactions")]
    [PrimaryKey("TransactionID")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        [Column(TypeName = "money")]
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public long Source_acc { get; set; }

        [Required]
        public long Dest_acc { get; set; }
    }
    public class BankingAppDbContext : DbContext
        {
            public DbSet<Account> Accounts { get; set; }
            public DbSet<Branch> Branches { get; set; }
            public DbSet<AccountType> AccountTypes { get; set; }

             public DbSet<Transaction> Transactions {  get; set; }
           public DbSet<Beneficiary> Beneficiaries {  get; set; }
   
        public DbSet<User> Users {  get; set; }

        public DbSet<DocType> DocTypes { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Customer> Customers {  get; set; }

        public DbSet<Role> Roles {  get; set; }

        public BankingAppDbContext(DbContextOptions<BankingAppDbContext> options)
         : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.AccountId)
                .UseIdentityColumn(); // or .ValueGeneratedOnAdd() depending on EF version
                                      // Other configurations
        }

    }

}
