using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;

namespace WallsShop.Repository;

public class FormRepository(WallShopContext ctx)
{ 
    public async Task<bool> CreateForm(FormDto form)
    {
        var formData = new Form
        {
            UserAddress =  string.IsNullOrWhiteSpace(form.Address) ? "No Address" : form.Address,
            PhoneNumber = form.PhoneNumber,
            Message =form.Message,
            Date =  new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
        };

        try
        {
            ctx.Forms.Add(formData);
            await ctx.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        } 
    }
}