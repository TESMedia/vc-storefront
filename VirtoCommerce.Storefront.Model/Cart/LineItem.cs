﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class LineItem : Entity, IDiscountable, IValidatable, ITaxable
    {
        public LineItem(Currency currency, Language language)
        {
            Currency = currency;
            LanguageCode = language.CultureName;
            ListPrice = new Money(currency);
            SalePrice = new Money(currency);
            ListPriceWithTax = new Money(currency);
            SalePriceWithTax = new Money(currency);
            TaxTotal = new Money(currency);

            Discounts = new List<Discount>();
            TaxDetails = new List<TaxDetail>();
            DynamicProperties = new List<DynamicProperty>();
            ValidationErrors = new List<ValidationError>();
            ValidationWarnings = new List<ValidationError>();
        }
        public string Balance { get; set; }
        /// <summary>
        /// Gets or sets line item created date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the product corresponding to line item
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the value of product id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the type of product (can be Physical, Digital or Subscription)
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the value of catalog id
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the value of category id
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the value of product SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the value of line item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of line item quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the value of line item warehouse location
        /// </summary>
        public string WarehouseLocation { get; set; }

        /// <summary>
        /// Gets or sets the value of line item shipping method code
        /// </summary>
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the requirement for line item shipping
        /// </summary>
        public bool RequiredShipping { get; set; }

        /// <summary>
        /// Gets or sets the value of line item thumbnail image absolute URL
        /// </summary>
        public string ThumbnailImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the value of line item image absolute URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the flag of line item is a gift
        /// </summary>
        public bool IsGift { get; set; }

        /// <summary>
        /// Gets or sets the value of language code
        /// </summary>
        /// <value>
        /// Culture name in ISO 3166-1 alpha-3 format
        /// </value>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Gets or sets the value of line item comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the flag of line item is recurring
        /// </summary>
        public bool IsReccuring { get; set; }

        /// <summary>
        /// Gets or sets flag of line item has tax
        /// </summary>
        public bool TaxIncluded { get; set; }

        /// <summary>
        /// Gets or sets the value of line item volumetric weight
        /// </summary>
        public decimal? VolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the value of line item weight unit
        /// </summary>
        public string WeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of line item weight
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the value of line item measurement unit
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of line item height
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the value of line item length
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// Gets or sets the value of line item width
        /// </summary>
        public decimal? Width { get; set; }

        /// <summary>
        /// Gets or sets the value of line item original price
        /// </summary>
        public Money ListPrice { get; set; }

        private Money _listPrice;
        /// <summary>
        /// Gets or sets the value of line item original price including tax 
        /// </summary>
        public Money ListPriceWithTax
        {
            get
            {
                return _listPrice ?? ListPrice;
            }
            set
            {
                _listPrice = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of line item sale price (include static discount)
        /// </summary>
        public Money SalePrice { get; set; }

        private Money _salePriceWithTax;
        /// <summary>
        /// Gets or sets the value of line item sale price (include static discount) including tax
        /// </summary>
        public Money SalePriceWithTax
        {
            get
            {
                return _salePriceWithTax ?? SalePrice;
            }
            set
            {
                _salePriceWithTax = value;
            }
        }

        /// <summary>
        /// Gets the value of line item actual price (include all types of discounts)
        /// </summary>
        public Money PlacedPrice
        {
            get
            {
                return ListPrice - ItemDiscount;
            }
        }

        public Money PlacedPriceWithTax
        {
            get
            {
                return ListPriceWithTax - ItemDiscountWithTax;
            }
        }

        /// <summary>
        /// Gets the value of line item subtotal price (actual price * line item quantity)
        /// </summary>
        public Money ExtendedPrice
        {
            get
            {
                return PlacedPrice * Quantity;
            }
        }

        public Money ExtendedPriceWithTax
        {
            get
            {
                return PlacedPriceWithTax * Quantity;
            }
        }

      
    
        /// <summary>
        /// Gets the value of the single qty line item discount amount
        /// </summary>
        public Money ItemDiscount
        {
            get
            {
                var retVal = ListPrice - SalePrice;
                if (!Discounts.IsNullOrEmpty())
                {
                    retVal += Discounts.Where(d => d != null).Sum(d => d.Amount.Amount);
                }

                return retVal;
            }
        }

        public Money ItemDiscountWithTax
        {
            get
            {
                var retVal = ListPriceWithTax - SalePriceWithTax;
                if (!Discounts.IsNullOrEmpty())
                {
                    retVal += Discounts.Where(d => d != null).Sum(d => d.AmountWithTax.Amount);
                }
                return retVal;
            }
        }

        /// <summary>
        /// Gets the value of line item total discount amount
        /// </summary>
        public Money DiscountTotal
        {
            get
            {
                return ItemDiscount * Quantity;
            }
        }

        public Money DiscountTotalWithTax
        {
            get
            {
                return ItemDiscountWithTax * Quantity;
            }
        }

        /// <summary>
        /// Used for dynamic properties management, contains object type string
        /// </summary>
        /// <value>Used for dynamic properties management, contains object type string</value>
        public string ObjectType { get; set; }

        /// <summary>
        /// Dynamic properties collections
        /// </summary>
        /// <value>Dynamic properties collections</value>
        public ICollection<DynamicProperty> DynamicProperties { get; set; }

        /// <summary>
        /// Gets or sets the cart validation type
        /// </summary>
        public ValidationType ValidationType { get; set; }

        public ICollection<ValidationError> ValidationErrors { get; set; }

        public ICollection<ValidationError> ValidationWarnings { get; set; }

        #region ITaxable Members
        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping tax type
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the collection of line item tax details lines
        /// </summary>
        /// <value>
        /// Collection of TaxDetail objects
        /// </value>
        public ICollection<TaxDetail> TaxDetails { get; set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            ListPriceWithTax = ListPrice;
            SalePriceWithTax = SalePrice;

            //Because TaxLine.Id may contains composite string id & extra info
            var lineItemTaxRates = taxRates.Where(x => x.Line.Id.SplitIntoTuple('&').Item1 == Id);
            TaxTotal = new Money(Currency);
            if (lineItemTaxRates.Any())
            {
                var extendedPriceRate = lineItemTaxRates.First(x => x.Line.Id.SplitIntoTuple('&').Item2.EqualsInvariant("extended"));
                var listPriceRate = lineItemTaxRates.First(x => x.Line.Id.SplitIntoTuple('&').Item2.EqualsInvariant("list"));
                var salePriceRate = lineItemTaxRates.FirstOrDefault(x => x.Line.Id.SplitIntoTuple('&').Item2.EqualsInvariant("sale"));
                if (salePriceRate == null)
                {
                    salePriceRate = listPriceRate;
                }
                TaxTotal += extendedPriceRate.Rate;
                ListPriceWithTax = ListPrice + listPriceRate.Rate;
                SalePriceWithTax = SalePrice + salePriceRate.Rate;
            }          
        }
        #endregion


        #region IDiscountable  Members
        public Currency Currency { get; private set; }

        public ICollection<Discount> Discounts { get; private set; }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var lineItemRewards = rewards.Where(r => r.RewardType == PromotionRewardType.CatalogItemAmountReward && (r.ProductId.IsNullOrEmpty() || r.ProductId.EqualsInvariant(ProductId)));
            if (lineItemRewards == null)
            {
                return;
            }

            Discounts.Clear();

            foreach (var reward in lineItemRewards)
            {
                var discount = reward.ToDiscountModel(SalePrice, SalePriceWithTax);
                if(reward.Quantity > 0)
                {
                    var money = discount.Amount * Math.Min(reward.Quantity, Quantity);
                    var withTaxMoney = discount.AmountWithTax * Math.Min(reward.Quantity, Quantity);
                    //TODO: need allocate more rightly between each quantities
                    discount.Amount = money.Allocate(Quantity).FirstOrDefault();
                    discount.AmountWithTax = withTaxMoney.Allocate(Quantity).FirstOrDefault();
                }
                if (reward.IsValid)
                {
                    Discounts.Add(discount);
                }
            }
        }
        #endregion

        public override string ToString()
        {
            return string.Format("cart lineItem #{0} {1} qty: {2}", Id ?? "undef", Name ?? "undef", Quantity);
        }
    }
}