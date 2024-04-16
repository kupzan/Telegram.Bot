using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

#nullable enable

namespace Telegram.Bot.Tests.Integ.Payments;

public class PaymentsBuilder
{
    readonly List<ShippingOption> _shippingOptions = new();
    Product? _product;
    string? _currency;
    string? _startParameter;
    string? _payload;
    long? _chatId;
    bool? _needName;
    bool? _needEmail;
    bool? _needShippingAddress;
    bool? _needPhoneNumber;
    bool? _isFlexible;
    bool? _sendEmailToProvider;
    bool? _sendPhoneNumberToProvider;
    string? _providerData;
    InlineKeyboardMarkup? _replyMarkup;
    string? _paymentsProviderToken;
    int? _maxTipAmount;
    int[]? _suggestedTipAmounts;

    public PaymentsBuilder WithCurrency(string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        _currency = currency;
        return this;
    }

    public PaymentsBuilder WithStartParameter(string startParameter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startParameter);
        _startParameter = startParameter;
        return this;
    }

    public PaymentsBuilder WithPayload(string payload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        _payload = payload;
        return this;
    }

    public PaymentsBuilder WithProviderData(string providerData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerData);

        _providerData = providerData;
        return this;
    }

    public PaymentsBuilder WithReplyMarkup(InlineKeyboardMarkup replyMarkup)
    {
        ArgumentNullException.ThrowIfNull(replyMarkup);
        _replyMarkup = replyMarkup;
        return this;
    }

    public PaymentsBuilder ToChat(long chatId)
    {
        _chatId = chatId;
        return this;
    }

    public PaymentsBuilder RequireName(bool require = true)
    {
        _needName = require;
        return this;
    }

    public PaymentsBuilder RequireEmail(bool require = true)
    {
        _needEmail = require;
        return this;
    }

    public PaymentsBuilder RequirePhoneNumber(bool require = true)
    {
        _needPhoneNumber = require;
        return this;
    }

    public PaymentsBuilder RequireShippingAddress(bool require = true)
    {
        _needShippingAddress = require;
        return this;
    }

    public PaymentsBuilder WithFlexible(bool value = true)
    {
        _isFlexible = value;
        return this;
    }

    public PaymentsBuilder SendEmailToProvider(bool send = true)
    {
        _sendEmailToProvider = send;
        return this;
    }

    public PaymentsBuilder SendPhoneNumberToProvider(bool send = true)
    {
        _sendPhoneNumberToProvider = send;
        return this;
    }

    public PaymentsBuilder WithPaymentProviderToken(string paymentsProviderToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentsProviderToken);
        _paymentsProviderToken = paymentsProviderToken;
        return this;
    }

    public PaymentsBuilder WithMaxTip(int maxTipAmount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxTipAmount, 1);
        if (_suggestedTipAmounts is not null && _suggestedTipAmounts.Any(tip => tip > maxTipAmount))
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxTipAmount),
                maxTipAmount,
                "Max tip is larger than some of the suggested tips"
            );
        }

        _maxTipAmount = maxTipAmount;

        return this;
    }

    public PaymentsBuilder WithSuggestedTips(params int[] suggestedTipAmounts)
    {
        ArgumentOutOfRangeException.ThrowIfZero(suggestedTipAmounts.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(suggestedTipAmounts.Length, 4);

        if (suggestedTipAmounts.Any(tip => tip < 1))
            throw new ArgumentException("Suggested tips must be greater than 0");

        if (_maxTipAmount is not null && suggestedTipAmounts.Any(tip => tip < _maxTipAmount))
            throw new ArgumentException("Suggested tips must not be greater than max tip amount");

        _suggestedTipAmounts = suggestedTipAmounts.OrderBy(tip => tip).ToArray();

        return this;
    }

    public PreliminaryInvoice GetPreliminaryInvoice()
    {
        if (_product is null) throw new InvalidOperationException("Product wasn't added");
        if (string.IsNullOrWhiteSpace(_currency)) throw new InvalidOperationException("Currency isn't set");

        return new()
        {
            Title = _product.Title,
            Currency = _currency,
            StartParameter = _startParameter,
            TotalAmount = _product.ProductPrices.Sum(price => price.Amount),
            Description = _product.Description,
        };
    }

    public Shipping GetShippingOptions() => new(_shippingOptions.ToArray());

    public int GetTotalAmount() =>
        (_product?.ProductPrices.Sum(price => price.Amount) ?? 0) +
        _shippingOptions.Sum(x => x.Prices.Sum(p => p.Amount));

    public int GetTotalAmountWithoutShippingCost() => _product?.ProductPrices.Sum(price => price.Amount) ?? 0;

    public SendInvoiceRequest BuildInvoiceRequest()
    {
        ArgumentNullException.ThrowIfNull(_product);
        ArgumentException.ThrowIfNullOrWhiteSpace(_paymentsProviderToken);
        ArgumentNullException.ThrowIfNull(_chatId);
        ArgumentException.ThrowIfNullOrWhiteSpace(_currency);
        ArgumentException.ThrowIfNullOrWhiteSpace(_payload);

        return new()
        {
            ChatId = _chatId.Value,
            Title = _product.Title,
            Description = _product.Description,
            Payload = _payload,
            ProviderToken = _paymentsProviderToken,
            Currency = _currency,
            Prices = _product.ProductPrices,
            PhotoUrl = _product.PhotoUrl,
            PhotoWidth = _product.PhotoWidth,
            PhotoHeight = _product.PhotoHeight,
            NeedShippingAddress = _needShippingAddress,
            IsFlexible = _isFlexible,
            NeedName = _needName,
            NeedEmail = _needEmail,
            NeedPhoneNumber = _needPhoneNumber,
            SendEmailToProvider = _sendEmailToProvider,
            SendPhoneNumberToProvider = _sendPhoneNumberToProvider,
            StartParameter = _startParameter,
            ProviderData = _providerData,
            ReplyMarkup = _replyMarkup,
            MaxTipAmount = _maxTipAmount,
            SuggestedTipAmounts = _suggestedTipAmounts
        };
    }

    public AnswerShippingQueryRequest BuildShippingQueryRequest(string shippingQueryId, string? errorMessage = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(shippingQueryId);

        AnswerShippingQueryRequest shippingQueryRequest = errorMessage is null
            ? new(shippingQueryId, _shippingOptions)
            : new(shippingQueryId, errorMessage);

        return shippingQueryRequest;
    }

    public PaymentsBuilder WithProduct(Action<ProductBuilder> builder)
    {
        ProductBuilder productBuilder = new();
        builder(productBuilder);

        _product = productBuilder.Build();

        return this;
    }

    public PaymentsBuilder WithShipping(Action<ShippingOptionsBuilder> config)
    {
        ShippingOptionsBuilder builder = new();
        config(builder);

        _shippingOptions.Add(builder.Build());

        return this;
    }

    public class ShippingOptionsBuilder
    {
        string? _id;
        string? _title;
        readonly List<LabeledPrice> _shippingPrices = new();

        public ShippingOptionsBuilder WithId(string id)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            _id = id;
            return this;
        }

        public ShippingOptionsBuilder WithTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            _title = title;
            return this;
        }

        public ShippingOptionsBuilder WithPrice(string label, int amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

            _shippingPrices.Add(new(label, amount));

            return this;
        }

        public ShippingOption Build() =>
            new()
            {
                Id = _id ?? throw new InvalidOperationException("Id is null"),
                Title = _title ?? throw new InvalidOperationException("Title is null"),
                Prices = _shippingPrices.Any()
                    ? _shippingPrices.ToArray()
                    : throw new InvalidOperationException("Shipping prices are empty")
            };
    }

    public class ProductBuilder
    {
        string? _description;
        string? _title;
        string? _photoUrl;
        int _photoWidth;
        int _photoHeight;
        readonly List<LabeledPrice> _productPrices = new();

        public ProductBuilder WithTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            _title = title;
            return this;
        }

        public ProductBuilder WithDescription(string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            _description = description;
            return this;
        }

        public ProductBuilder WithPhoto(string url, int width, int height)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url);
            ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);

            _photoUrl = url;
            _photoWidth = width;
            _photoHeight = height;

            return this;
        }

        public ProductBuilder WithProductPrice(string label, int amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            _productPrices.Add(new(label, amount));
            return this;
        }

        public Product Build() =>
            new()
            {
                Title = _title ?? throw new InvalidOperationException("Title is null"),
                Description = _description  ?? throw new InvalidOperationException("Description is null"),
                PhotoUrl = _photoUrl,
                PhotoHeight = _photoHeight,
                PhotoWidth = _photoWidth,
                ProductPrices = _productPrices.Any()
                    ? _productPrices.ToArray()
                    : throw new InvalidOperationException("Prices are empty")
            };
    }

    public record Product
    {
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? PhotoUrl { get; init; }
        public int PhotoWidth { get; init; }
        public int PhotoHeight { get; init; }
        public IReadOnlyCollection<LabeledPrice> ProductPrices { get; init; } = default!;
    }

    public record PreliminaryInvoice
    {
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string? StartParameter { get; init; }
        public string Currency { get; init; } = default!;
        public int TotalAmount { get; init; }
    }

    public record Shipping(IReadOnlyList<ShippingOption> ShippingOptions)
    {
        public int TotalAmount => ShippingOptions.Sum(x => x.Prices.Sum(p => p.Amount));
    }
}
