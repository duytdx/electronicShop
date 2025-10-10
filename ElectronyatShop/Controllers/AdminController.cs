using ElectronyatShop.Data;
using ElectronyatShop.Models;
using ElectronyatShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ElectronyatShop.Enums;

namespace ElectronyatShop.Controllers;

[Authorize("AdminRole")]
public class AdminController : Controller
{
	#region Controller Constructor and Attributes

	private ElectronyatShopDbContext Context { get; set; }
	private UserManager<ApplicationUser> UserManager { get; set; }

	public AdminController(ElectronyatShopDbContext context, UserManager<ApplicationUser> userManager)
	{
		Context = context;
		UserManager = userManager;
	}

	#endregion

	#region Controller Actions

	[HttpGet]
	public async Task<IActionResult> Index() => View("Index", await Context.Products.AsNoTracking().ToListAsync());

	[HttpGet]
	public IActionResult AddNewProduct()
	{
		ProductViewModel productViewModel = new();
		FillSelectedListForProductStatus(productViewModel);
		return View("New", productViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> AddNewProduct([FromForm] ProductViewModel productViewModel)
	{
		if (!ModelState.IsValid || productViewModel.Image is null || productViewModel.Image.Length == 0)
		{
			FillSelectedListForProductStatus(productViewModel);
			return View("New", productViewModel);
		}
		var product = new Product
		{
			Name = productViewModel.Name,
			Image = ProcessUploadedImage(productViewModel),
			Type = productViewModel.ProductType,
			Description = productViewModel.Description,
			Price = productViewModel.Price,
			AvailableQuantity = productViewModel.AvailableQuantity,
			DiscountPercentage = productViewModel.DiscountPercentage,
			Status = productViewModel.Status
		};
		await Context.Products.AddAsync(product);
		await Context.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> EditProduct([FromRoute] int id)
	{
		var product = await Context.Products.FindAsync(id);
		if (product is null)
			return RedirectToAction("Index");

		var productViewModel = new ProductViewModel
		{
			Id = product.Id,
			Name = product.Name,
			ImageName = product.Image,
			ProductType = product.Type,
			Description = product.Description,
			Price = product.Price,
			AvailableQuantity = product.AvailableQuantity,
			DiscountPercentage = product.DiscountPercentage,
			Status = product.Status
		};
		FillSelectedListForProductStatus(productViewModel);
		return View("Edit", productViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> EditProduct([FromForm] ProductViewModel productViewModel)
	{
		var product = await Context.Products.FindAsync(productViewModel.Id);
		if (!ModelState.IsValid || product is null || product.Id != productViewModel.Id)
		{
			FillSelectedListForProductStatus(productViewModel);
			return View("Edit", productViewModel);
		}
		product.Name = productViewModel.Name;
		product.Type = productViewModel.ProductType;
		product.Description = productViewModel.Description;
		product.Price = productViewModel.Price;
		product.AvailableQuantity = productViewModel.AvailableQuantity;
		product.DiscountPercentage = productViewModel.DiscountPercentage;
		product.Status = productViewModel.Status;
		if (productViewModel.Image is not null)
			product.Image = ProcessUploadedImage(productViewModel);

		Context.Products.Update(product);
		await Context.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> DeleteProduct(int id)
	{
		var product = await Context.Products.FindAsync(id);
		if (product is null) return RedirectToAction("Index");

		var imageName = product.Image;
		Context.Products.Remove(product);
		await Context.SaveChangesAsync();
		DeleteImage(imageName);
		return RedirectToAction("Index");
	}

	// User Management Actions
	[HttpGet]
	public async Task<IActionResult> Users()
	{
		var users = await UserManager.Users.ToListAsync();
		return View("Users", users);
	}

	[HttpGet]
	public async Task<IActionResult> UserDetails(string id)
	{
		var user = await UserManager.FindByIdAsync(id);
		if (user == null) return RedirectToAction("Users");
		
		var roles = await UserManager.GetRolesAsync(user);
		ViewBag.Roles = roles;
		return View("UserDetails", user);
	}

	[HttpPost]
	public async Task<IActionResult> ToggleUserRole(string userId)
	{
		var user = await UserManager.FindByIdAsync(userId);
		if (user == null) return RedirectToAction("Users");

		var currentRoles = await UserManager.GetRolesAsync(user);
		
		if (currentRoles.Contains("AdminRole"))
		{
			await UserManager.RemoveFromRoleAsync(user, "AdminRole");
			await UserManager.AddToRoleAsync(user, "CustomerRole");
		}
		else
		{
			await UserManager.RemoveFromRoleAsync(user, "CustomerRole");
			await UserManager.AddToRoleAsync(user, "AdminRole");
		}

		return RedirectToAction("Users");
	}

	// Order Management Actions
	[HttpGet]
	public async Task<IActionResult> Orders()
	{
		var orders = await Context.Orders
			.Include(o => o.User)
			.Include(o => o.OrderItems!)
			.ThenInclude(oi => oi.Product)
			.OrderByDescending(o => o.CreateDate)
			.ToListAsync();
		return View("Orders", orders);
	}

	[HttpGet]
	public async Task<IActionResult> OrderDetails(int id)
	{
		var order = await Context.Orders
			.Include(o => o.User)
			.Include(o => o.OrderItems!)
			.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(o => o.Id == id);

		if (order == null) return RedirectToAction("Orders");
		return View("OrderDetails", order);
	}

	[HttpPost]
	public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
	{
		var order = await Context.Orders.FindAsync(orderId);
		if (order == null) return RedirectToAction("Orders");

		order.Status = status;
		Context.Orders.Update(order);
		await Context.SaveChangesAsync();

		return RedirectToAction("OrderDetails", new { id = orderId });
	}

	#endregion

	#region Controller Logic

	/// <summary>
	/// Save the chosen Image on creating to the Image root
	/// </summary>
	/// <param name="productViewModel">ProductViewModel</param>
	/// <returns>Image Name</returns>
	private static string ProcessUploadedImage(ProductViewModel productViewModel)
	{
		var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
		var imageName = $"{DateTime.Now:dddd-MMM-dd-yyyy-hh-mm-ss}-{productViewModel.Image?.FileName}";
		var imagePath = Path.Combine(imagesDirectory, imageName);

		var stream = new FileStream(imagePath, FileMode.Create);
		productViewModel.Image?.CopyTo(stream);

		return imageName;
	}

	private static void DeleteImage(string imageName)
	{
		var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
		var imagePath = Path.Combine(imagesDirectory, imageName);
		if (System.IO.File.Exists(imagePath))
			System.IO.File.Delete(imagePath);
	}

	private static void FillSelectedListForProductStatus(ProductViewModel productViewModel)
	{
		List<SelectListItem> items =
		[
			new() { Value = "True", Text = "Yes" },
			new() { Value = "False", Text = "No" }
		];
		productViewModel.StatusList = new SelectList(items, "Value", "Text");
	}

	#endregion
}