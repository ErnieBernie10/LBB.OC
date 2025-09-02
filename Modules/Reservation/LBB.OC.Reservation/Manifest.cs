using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "LBB.OC.Reservation",
    Author = "Little Big Biz",
    Website = "",
    Version = "0.0.1",
    Description = "LBB.OC.Reservation",
    Category = "Content Management",
    Dependencies = new[]
    {
        "OrchardCore.Resources", "OrchardCore.Email", "OrchardCore.Tenants", "OrchardCore.Email.Smtp",
        "OrchardCore.Notifications", "OrchardCore.Notifications.Email", "OrchardCore.BackgroundTasks"
    }
)]