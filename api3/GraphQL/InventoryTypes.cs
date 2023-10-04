using api3.Models;
using GreenDonut;

namespace api3.GraphQL
{

    public class InventoryInput
    {
        public ShopInput Shop { get; set; }
        public EmployeeInput Employee { get; set; }
        public IcecreamInput Icecream { get; set; }
        public string Date { get; set; }
    }

    public class IcecreamInput
    {
        public string Flavor { get; set; }
        public int Count { get; set; }
        public bool IsSeasonFlavor { get; set; }
    }

    public class ShopInput
    {
        public string Name { get; set; }
    }

    public class EmployeeInput
    {
        public string Name { get; set; }
    }



    public class IcecreamType : ObjectType<Inventory>
    {
        protected override void Configure(IObjectTypeDescriptor<Inventory> descriptor)
        {
            descriptor.Field(i => i.Flavor).Type<StringType>();
            descriptor.Field(i => i.Quantity).Type<IntType>().Name("count");
            descriptor.Field(i => i.IsSeasonFlavor).Type<BooleanType>()
                .Resolver(context =>
                {
                    var isSeasonFlavor = context.Parent<Inventory>().IsSeasonFlavor;
                    if (isSeasonFlavor is bool)
                    {
                        return isSeasonFlavor;
                    }
                    else if (isSeasonFlavor is string)
                    {
                        return ((string)isSeasonFlavor).Equals("Yes", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        return false; // Valor predeterminado si no es booleano ni una cadena "Yes"
                    }
                });
        }
    }




    public class Query
    {
        [UseProjection]
        [HotChocolate.Data.UseFiltering]
        [HotChocolate.Data.UseSorting]
        public IQueryable<Inventory> GetInventory([Service] PgAdminContext dbContext) =>
            dbContext.Inventories;

        public IQueryable<Inventory> InventoryByFlavor([Service] PgAdminContext dbContext, string flavor) =>
            dbContext.Inventories.Where(i => i.Flavor == flavor);

        public IQueryable<Inventory> InventoryByShop([Service] PgAdminContext dbContext, string shop) =>
            dbContext.Inventories.Where(i => i.IdStoreNavigation.Name == shop);

        public IQueryable<Inventory> InventoryByDate([Service] PgAdminContext dbContext, string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                //formato UTC
                parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);

                return dbContext.Inventories.Where(i => i.Date == parsedDate.Date);
            }
            else
            {
                // Solo por ciaca
                return Enumerable.Empty<Inventory>().AsQueryable();
            }
        }



    }

    public class Mutation
    {
        public Inventory AddInventory([Service] PgAdminContext dbContext, InventoryInput input)
        {
            // (porque no tengo identity)
            var lastInventory = dbContext.Inventories
    .OrderByDescending(e => e.IdInventory)
    .FirstOrDefault();

            int id = 0;

            if (lastInventory != null)
            {
                id = Convert.ToInt32(lastInventory.IdInventory + 1);
            }
            else
            {
                id = 1;
            }


            string employeeName = input.Employee.Name;
            int idEmployee = dbContext.Employees
     .Where(employee => employee.Name == employeeName)
     .Select(employee => employee.IdEmployee)
     .FirstOrDefault();

            string shopName = input.Shop.Name;
            int idStore = dbContext.Stores
    .Where(store => store.Name == shopName)
    .Select(store => store.IdStore)
    .FirstOrDefault();


            string is_season_flavor;
            if (input.Icecream.IsSeasonFlavor)
            {
                is_season_flavor = "Yes";
            }
            else
            {
                is_season_flavor = "No";
            }

            // Crea una nueva instancia de Inventory a partir de los datos proporcionados en input
            var newInventory = new Inventory
            {
                IdInventory = id,
                IdEmployee = idEmployee,
                IdStore = idStore,
                Flavor = input.Icecream.Flavor,
                Quantity = input.Icecream.Count,
                IsSeasonFlavor = is_season_flavor,
                Date = DateTime.UtcNow // Utiliza la fecha y hora actual en formato UTC
            };

            // Agrega el nuevo inventario a la base de datos
            dbContext.Inventories.Add(newInventory);
            dbContext.SaveChanges();

            // Devuelve el inventario recién creado
            return newInventory;
        }
    }
}