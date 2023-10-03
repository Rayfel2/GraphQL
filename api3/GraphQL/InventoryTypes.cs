using api3.Models;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

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
    

    public class InventoryType : ObjectType<Inventory>
{
    protected override void Configure(IObjectTypeDescriptor<Inventory> descriptor)
    {
        descriptor.Field(i => i.IdInventory).Type<IdType>();
        descriptor.Field(i => i.IdStore).Type<IdType>();
        descriptor.Field(i => i.IdEmployee).Type<IdType>();
        descriptor.Field(i => i.Date).Type<DateTimeType>();
        descriptor.Field(i => i.Flavor).Type<StringType>();
        descriptor.Field(i => i.IsSeasonFlavor).Type<BooleanType>();
        descriptor.Field(i => i.Quantity).Type<IntType>();
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
                // Asegurarse de que la fecha sea en formato UTC
                parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);

                return dbContext.Inventories.Where(i => i.Date == parsedDate.Date);
            }
            else
            {
                // Manejo de error si la conversión de fecha falla
                return Enumerable.Empty<Inventory>().AsQueryable();
            }
        }



    }

    public class Mutation
    {
        public Inventory AddInventory([Service] PgAdminContext dbContext, InventoryInput input)
        {
            var lastInventory = dbContext.Inventories
    .OrderByDescending(e => e.IdInventory)
    .FirstOrDefault();

            int id = 0;

            if (lastInventory != null)
            {
                // Se encontró el último registro, devuelve el siguiente ID.
                id = Convert.ToInt32(lastInventory.IdInventory + 1);
                
            }
            else
            {
                id = 1;
                // No se encontraron registros en la tabla, devuelve 1 como el primer ID.

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

            // Crea una nueva instancia de Inventory a partir de los datos proporcionados en input
            var newInventory = new Inventory
            {
                IdInventory = id,
                IdEmployee = idEmployee,
                IdStore = idStore,
                Flavor = input.Icecream.Flavor,
                Quantity = input.Icecream.Count,
                IsSeasonFlavor = Convert.ToString(input.Icecream.IsSeasonFlavor),
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