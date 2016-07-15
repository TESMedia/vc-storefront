using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class LineItemConverter
    {
        public static LineItem ToShopifyModel(this StorefrontModel.Cart.LineItem lineItem, StorefrontModel.WorkContext workContext)
        {
            var shopifyModel = new LineItem();

            //shopifyModel.Product = lineItem.Product.ToShopifyModel();
            shopifyModel.Fulfillment = null; // TODO
            shopifyModel.Grams = lineItem.Weight ?? 0m;
            shopifyModel.Id = lineItem.Id;
            shopifyModel.Image = new Image
            {
                Alt = lineItem.Name,
                Name = lineItem.Name,
                ProductId = lineItem.ProductId,
                Src = lineItem.ImageUrl
            };
            shopifyModel.LinePrice = lineItem.ExtendedPrice.Amount * 100;
            shopifyModel.LinePriceWithTax = lineItem.ExtendedPriceWithTax.Amount * 100;
            shopifyModel.Price = lineItem.PlacedPrice.Amount * 100;
            shopifyModel.PriceWithTax = lineItem.PlacedPriceWithTax.Amount * 100;
            shopifyModel.ProductId = lineItem.ProductId;
            //shopifyModel.Properties = null; // TODO
            shopifyModel.Quantity = lineItem.Quantity;
            shopifyModel.RequiresShipping = lineItem.RequiredShipping;
            shopifyModel.Sku = lineItem.Sku;
            shopifyModel.Taxable = lineItem.TaxIncluded;
            shopifyModel.Title = lineItem.Name;
            shopifyModel.Type = null; // TODO
            shopifyModel.Url = null; // TODO
            shopifyModel.Variant = null; // TODO
            shopifyModel.VariantId = lineItem.ProductId;
            shopifyModel.Vendor = null; // TODO

            return shopifyModel;
        }
        public static string Returnbalance()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://tls.tes.media/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/TLSMobileAuthentication/ShowPoint?Email=" + GetUserName()).Result;
            string obj = response.Content.ReadAsStringAsync().Result;
            return obj;
        }
    
        public static LineItem ToShopifyModel(this StorefrontModel.Order.LineItem lineItem, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new LineItem
            {
                Fulfillment = null,
                Grams = lineItem.Weight ?? 0m,
                Id = lineItem.Id,
                Quantity = lineItem.Quantity ?? 0,
                Price = lineItem.Price.Amount * 100,
                PriceWithTax = lineItem.PriceWithTax.Amount * 100,
                ProductId = lineItem.ProductId,
                Sku = lineItem.Name,
                Title = lineItem.Name,
                Balance = Returnbalance(),
                Url = urlBuilder.ToAppAbsolute("/product/" + lineItem.ProductId),
            };

            result.LinePrice = result.Price * result.Quantity;
            result.LinePriceWithTax = result.PriceWithTax * result.Quantity;

            result.Product = new Product
            {
                Id = result.ProductId,
                Url = result.Url
            };

            //result.Image = lineItem.Product.PrimaryImage != null ? lineItem.Product.PrimaryImage.ToShopifyModel() : null;
            //result.RequiresShipping = lineItem.RequiredShipping;
            //result.Taxable = lineItem.TaxIncluded;

            return result;
        }
        public static string GetUserName()
        {
            var context = HttpContext.Current.GetOwinContext().Request;
           
            if (context.User.Identity.IsAuthenticated)
            {
                var userCtx = HttpContext.Current.GetOwinContext();
                var user = userCtx.Request.User.Identity.GetUserName();
                return context.User.Identity.GetUserName();
            }
            return string.Empty;


        }

    }
}