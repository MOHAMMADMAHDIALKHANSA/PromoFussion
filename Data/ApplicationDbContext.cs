using MarketingHub.Models;
using MarketingHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace MarketingHub.Data
{

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<MarketingAgency> MarketingAgency { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<MarketingAgencyRegistration> MarketingAgencyRegistrations { get; set; }
        public DbSet<Post> Posts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MarketingAgency>()
                .HasOne(a => a.applicationUser)
                .WithOne()
                .HasForeignKey<MarketingAgency>(a => a.UserId);
        
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MarketingAgencyRegistration>()
           .HasOne(mar => mar.Customer)
           .WithMany(c => c.MarketingAgencyRegistrations)
           .HasForeignKey(mar => mar.CustomerId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MarketingAgencyRegistration>()
                .HasOne(mar => mar.MarketingAgency)
                .WithMany(ma => ma.MarketingAgencyRegistrations)
                .HasForeignKey(mar => mar.MarketingAgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
               .HasOne(c => c.MarketingAgency)
               .WithMany(m => m.Customers)
               .HasForeignKey(c => c.MarketingAgencyId)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MarketingAgency>()
               .HasOne(m => m.Location)
               .WithOne()
               .HasForeignKey<MarketingAgency>(m => m.LocationId);



            modelBuilder.Entity<Post>()
             .HasOne(p => p.MarketingAgency)
             .WithMany(c => c.Post)
             .HasForeignKey(p => p.MarketingAgencyId)
             .OnDelete(DeleteBehavior.Cascade);

 //           modelBuilder.Entity<Location>().HasData(
 //             new Location
 //             {
 //                 LocationId = 1,
 //                 City = "Beirut",
 //                 ImageUrl = "https://www.macentreprise.com/img/projects/general-contracting/MHM%20Business%20Center%20-%20Antelias%20and%20Achrafieh%20-%20Adlieh/IMG_5730.jpg",
 //                 PostalCode = 51,
 //                 Region = "Jnah",
 //                 UrlRoute = "https://g.co/kgs/4xKBRqp",
 //                 BuildingName = "IDS ",
 //             },
 //             new Location
 //             {
 //                 LocationId = 2,
 //                 City = "Beirut",
 //                 ImageUrl = "https://www.yelleb.com/img/lb/j/1543234930-75-nascode.jpg",
 //                 PostalCode = 51,
 //                 Region = "Sin El Fil",
 //                 UrlRoute = "https://maps.app.goo.gl/XdgeL5jJim1GpW997",
 //                 BuildingName = "NASCOD",
 //             },
 //             new Location
 //             {
 //                 LocationId = 3,
 //                 City = "Beirut",
 //                 ImageUrl = "https://www.samwoh.com.sg/images/Home_Page/5-Latest-Event.jpg",
 //                 PostalCode = 51,
 //                 Region = "Horch Tabet",
 //                 UrlRoute = "https://maps.app.goo.gl/Pcjhs18qevw8UhAc7",
 //                 BuildingName = "Samwoh",
 //             },
 //             new Location
 //             {
 //                 LocationId = 4,
 //                 City = "Beirut",
 //                 ImageUrl = "https://thingsthatmatter.me/wp-content/uploads/2017/06/office.png",
 //                 PostalCode = 51,
 //                 Region = "Horsh Tabet",
 //                 UrlRoute = "https://maps.app.goo.gl/Q6Vpxm7iRvT2cVuJ7",
 //                 BuildingName = "Born Creative",
 //             }
 //          );

 //           modelBuilder.Entity<MarketingAgency>().HasData(
 //    new MarketingAgency
 //    {
 //        MarketingAgencyId = 1,
 //        Username = "BeyonFood",
 //        PhoneNumber = "987654",
 //        Email = "BeyondFood@gmail.com",
 //        Salary = "150$",
 //        Description = "Our main objective is to leverage our firsthand encounters with various restaurants and ventures, whether through physical visits or delivery services. We aim to provide insightful reviews of the food quality and overall service experience to assist individuals in finding establishments that offer exceptional meals. Concurrently, our efforts also extend to bolstering the endeavors of business owners, thereby contributing to the vitality of the local economy.",
 //        Rating = 4,
 //        ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSqBzemu65d0_r5XLDHA2yTwDMz_LZ_VdlsyAPozbIVVA&s",
 //        Instagram = "https://www.instagram.com/beyondfood.lb?igsh=MXdkYWRoOGZ5Nm41Mw==",
 //        Facebook = null,
 //        LinkedIn = null,
 //        Twitter = null,
 //        LocationId = 2,
 //    },
 //    new MarketingAgency
 //    {
 //        MarketingAgencyId = 2,
 //        Username = "Explorernico",
 //        PhoneNumber = "123456",
 //        Email = "Explorernico@gmail.com",
 //        Salary = "225$",
 //        Description = "Catering to the diverse needs of our clients, we specialize in crafting " +
 //        "innovative marketing strategies tailored to amplify brand" +
 //        " visibility and engagement. With a focus on driving impactful campaigns across multiple channels," +
 //        " we leverage our expertise to foster meaningful connections between brands and their target " +
 //        "audience. Our mission is to empower businesses to thrive in today's dynamic market landscape through" +
 //        " creative and data-driven marketing solutions.",
 //        Rating = 5,
 //        ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS1314X2xOaOYisxpppju84_U0mxInsp_3rYMX5KYjs2Tuk9m9xuvL7pUS_nal3vG7YZ8A&usqp=CAU",
 //        Instagram = "https://www.instagram.com/explorernico?igsh=MXd1cWRkMnN2bWtuNw==",
 //        Facebook = null,
 //        LinkedIn = null,
 //        Twitter = null,
 //        LocationId = 3,
 //    },
 //    new MarketingAgency
 //    {
 //        MarketingAgencyId = 3,
 //        Username = "Evolve",
 //        PhoneNumber = "741258",
 //        Email = "Evolve@gmail.com",
 //        Salary = "300$",
 //        Description = "Driven by a relentless pursuit of excellence, " +
 //        "our marketing agency specializes in delivering cutting-edge solutions that propel brands to new heights." +
 //        " With a team of seasoned professionals and a forward-thinking approach, we offer a comprehensive suite of services " +
 //        "designed to maximize brand impact and foster lasting connections with consumers. Partner with us to unlock your brand's " +
 //        "full potential and achieve unparalleled success in today's competitive landscape.",
 //        Rating = 5,
 //        ImageUrl = "https://media.licdn.com/dms/image/C560BAQGRK1Kf6VCE8A/company-logo_200_200/0/1678115779285/evolvebst_logo?e=2147483647&v=beta&t=zytDG9Btn9NVSL_LZ1UL67zOASMjHvW8sGiY8nmQmOg",
 //        Instagram = "https://www.instagram.com/evolve.pd?igsh=ZGV0ZDVzdTFxNm16",
 //        Facebook = null,
 //        LinkedIn = null,
 //        Twitter = null,
 //        LocationId = 4,
 //    },
 //    new MarketingAgency
 //    {
 //        MarketingAgencyId = 4,
 //        Username = "Food Couple",
 //        PhoneNumber = "852366",
 //        Email = "FoodCouple@gmail.com",
 //        Salary = "200$",
 //        Description = "We're dedicated to elevating brands through strategic storytelling and dynamic campaigns." +
 //        " With a keen eye for detail and a passion for creativity," +
 //        " we collaborate closely with our clients to bring their visions to life. From concept to execution," +
 //        " we're committed to delivering exceptional results that resonate with audiences and drive tangible business growth.",
 //        Rating = 5,
 //        ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT0bf9-VN81pZVNblRCFeLHWXo9IlZYxheinxmXtJ9k5X06olNmg5uXgnsZMHHCkd7LScU&usqp=CAU",
 //        Instagram = "https://www.instagram.com/foodcouplelb?igsh=bGdqYW5qam5ocW9n",
 //        Facebook = null,
 //        LinkedIn = null,
 //        Twitter = null,
 //        LocationId = 1
 //    }
 //);





        //    modelBuilder.Entity<Customer>().HasData(
        //      new Customer { CustomerId = "5", FirstName = "Scarlett", LastName = "Witch", Email = "Scarlet@gmail.com", PhoneNumber = "567894", MarketingAgencyId = null },
        //      new Customer { CustomerId = "6", FirstName = "John", LastName = "Wick", Email = "John@gmail.com", PhoneNumber = "124563", MarketingAgencyId = null },
        //      new Customer { CustomerId = "7", FirstName = "Rony", LastName = "Bang", Email = "Rony@gmail.com", PhoneNumber = "968523", MarketingAgencyId = null },
        //      new Customer { CustomerId = "8", FirstName = "Andy", LastName = "Beans", Email = "Andy@gmail.com", PhoneNumber = "951357", MarketingAgencyId = null }
        //        );

        //    modelBuilder.Entity<Administrator>().HasData(
        //        new Administrator { AdministratorId = 1, FirstName = "Mohammad Mahdi", LastName = "Al Khansaa", Email = "10121476@mu.edu.lb", Role = "Web Developer", ImgUrl = "https://t3.ftcdn.net/jpg/05/74/95/50/240_F_574955019_gk0EfaPbwaIzRf3HtS8KJdKmduOx8V5G.jpg" },
        //        new Administrator { AdministratorId = 2, FirstName = "Zahraa", LastName = "Kleit", Email = "10121528@mu.edu.lb", Role = "Web Developer", ImgUrl = "https://t4.ftcdn.net/jpg/05/56/30/03/240_F_556300316_yzdmBXMPGTslQfvuNJMbkC8x2yBbcp0u.jpg" }
        //        );





        //    modelBuilder.Entity<Post>().HasData(
        //      new Post
        //      {
        //          PostId = 1,
        //          Caption = "Steak House",
        //          Content = "~/assets2/img/SteakHouse.mp4",
        //          MarketingAgencyId = 1
        //      },
        //      new Post
        //      {
        //          PostId = 2,
        //          Caption = "Zen Baby Spa",
        //          Content = "~/assets2/img/ZenBabySpa.mp4",
        //          MarketingAgencyId = 1
        //      },
        //       new Post
        //       {
        //           PostId = 3,
        //           Caption = "AsalBee",
        //           Content = "~/assets/img/AsalBee.jpg",
        //           MarketingAgencyId = 3
        //       }, new Post
        //       {
        //           PostId = 4,
        //           Caption = "Authentic Italian Cuisine",
        //           Content = "~/assets2/img/ItalianHouse.jpg",
        //           MarketingAgencyId = 3
        //       }, new Post
        //       {
        //           PostId = 5,
        //           Caption = "Food Yard",
        //           Content = "~/assets2/img/FoodYard.mp4",
        //           MarketingAgencyId = 2
        //       }, new Post
        //       {
        //           PostId = 6,
        //           Caption = "Opale",
        //           Content = "~/assets2/img/opale.mp4",
        //           MarketingAgencyId = 2
        //       },
        //        new Post
        //        {
        //            PostId = 7,
        //            Caption = "Rachidi Group",
        //            Content = "~/assets2/img/RachidiGroup.mp4",
        //            MarketingAgencyId = 4
        //        }, new Post
        //        {
        //            PostId = 8,
        //            Caption = "Bob Cup",
        //            Content = "~/assets2/img/Bob.mp4",
        //            MarketingAgencyId = 4
        //        }

        //   );
        }

        internal string? FindAsync(string? customerId)
        {
            throw new NotImplementedException();
        }


    }
}