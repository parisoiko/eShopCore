// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// JavaScript source code
$('#searchBar').keypress(function (e) {
    var key = e.which;
    if (key === 13)  // the enter key code
    {
        vm.searchProduct();
    }
});

/* Normal Javascript */

//const str1 = 'hitting';
//const str2 = 'kitten';
//const levenshteinDistance = (str1 = '', str2 = '') => {
//    const track = Array(str2.length + 1).fill(null).map(() =>
//        Array(str1.length + 1).fill(null));
//    for (let i = 0; i <= str1.length; i += 1) {
//        track[0][i] = i;
//    }
//    for (let j = 0; j <= str2.length; j += 1) {
//        track[j][0] = j;
//    }
//    for (let j = 1; j <= str2.length; j += 1) {
//        for (let i = 1; i <= str1.length; i += 1) {
//            const indicator = str1[i - 1] === str2[j - 1] ? 0 : 1;
//            track[j][i] = Math.min(
//                track[j][i - 1] + 1, // deletion
//                track[j - 1][i] + 1, // insertion
//                track[j - 1][i - 1] + indicator, // substitution
//            );
//        }
//    }
//    return track[str2.length][str1.length];
//};

//const levenshteinMultiple = function (str, strSearch, steps) {
//    const myArray = str.split(" ");
//    for (var i = 0; i < myArray.length; i++) {
//        if (levenshteinDistance(myArray[i], strSearch) <= steps) {
//            return true;
//        }
//    }
//};

function formatCurrency(value) {
    return Number(value).toFixed(2) + "\u20AC";
}
/* View Models */
var productViewModel = function (product, onRemove, onSave) {
    var self = this;
    self.ID = ko.observable(product !== null ? product.id : 0).extend({ required: true, number: true });
    self.title = ko.observable(product !== null ? product.title : "").extend({ required: true, minLength: 5 });
    self.descriptionShort = ko.observable(product !== null ? product.descriptionShort : "").extend({ required: true, minLength: 5 });
    self.descriptionLong = ko.observable(product !== null ? product.descriptionLong : "").extend({ required: true, minLength: 5 });
    self.category = ko.observable(product !== null ? product.category : "").extend({ required: true, minLength: 5 });
    self.price = ko.observable(product !== null ? product.price : 0).extend({
        pattern: {
            params: /^\s*-?(\d+(\.\d{1,2})?|\.\d{1,2})\s*$/,
            message: "Invalid money format."
        }
    });
    self.imageSource = ko.observable(product !== null ? product.imageSource : "").extend({ required: true, minLength: 5 });
    self.manufacturer = ko.observable(product !== null ? product.manufacturer : "").extend({ required: true, minLength: 2 });
    self.quantity = ko.observable(product === null || product.quantity === undefined ? 0 : product.quantity);
    self.expanded = ko.observable(false);
    self.imgText = ko.observable("Alt text");

    self.onRemoveDelegate = onRemove;
    self.onSaveDelegate = onSave;

    self.inCart = ko.pureComputed(function () {
        return self.quantity() > 0;
    });
    self.totalPrice = ko.pureComputed(function () {
        return self.price() * self.quantity();
    });
    /* Functions */
    self.toggle = function () {
        self.expanded(!self.expanded());
    };

    self.addQuantity = function () {
        self.quantity(self.quantity() + 1);
        self.onSaveDelegate();
    }

    self.removeQuantity = function () {
        self.quantity(self.quantity() - 1);
        if (!self.inCart()) {
            self.onRemoveDelegate(self);
            self.onSaveDelegate();
        } else {
            self.onSaveDelegate();
        }
    }
    /* Ajax Posts to Server */
    self.editProduct = function () {
        $.ajax({
            type: 'POST',
            url: '/Products/EditProduct',
            data: ko.toJSON(self),
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                window.location.href = '/Products/Index';
            },
            error: function (data, success, error) {
                alert("Error : " + error);
            }
        });
    }

    self.deleteProduct = function () {
        $.ajax({
            type: 'POST',
            url: '/Products/DeleteConfirmed',
            data: ko.toJSON(self.ID()),
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                window.location.href = '/Products/Index';
            },
            error: function (data, success, error) {
                alert("Error : " + error);
            }
        });
    }

    self.createProduct = function () {
        console.log("im in create product");
        $.ajax({
            type: 'POST',
            url: '/Products/CreateProduct',
            data: ko.toJSON(self),
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                window.location.href = '/Products/Index';
            },
            error: function (data, success, error) {
                alert("Error : " + error);
            }
        });
    }
}

var generalViewModel = function (products, connection) {

    var self = this;
    self.connection = connection;
    /* Initiate Cart Data */
    var sessionData = [];
    if (sessionStorage.getItem('myShop') !== null) {
        sessionData = JSON.parse(sessionStorage.getItem('myShop'));
    }
    self.cartViewModel = new cartViewModel(sessionData);

    self.MergeSession = function () {
        for (var i = 0; i < self.cartViewModel.products().length; i++) {
            for (var j = 0; j < self.products().length; j++) {
                if (self.cartViewModel.products()[i].title() === self.products()[j].title()) {
                    self.products.replace(self.products()[j], self.cartViewModel.products()[i]);
                }
            }
        }
    }

    /* Constructor */
    self.products = ko.observableArray(ko.utils.arrayMap(products, function (product) {
        return new productViewModel(product, self.cartViewModel.removeFromCart, self.cartViewModel.setSessionStorage);
    }));

    self.categories = ko.observableArray();

    /* Functions */
    self.addProduct = function (product) {
        self.products.unshift(new productViewModel(product, cartViewModel.removeFromCart, cartViewModel.setSessionStorage));
    }

    self.removeProduct = function (product) {
        self.products.remove(function (value) { return value.ID() === product.id; });
    }

    self.addToCart = function (product) {
        self.cartViewModel.addToCart(product);
    }

    self.sidebarToggle = function () {
        $('#sidebar').toggleClass('active');
    }

    self.modalToggle = function () {
        $("#cardModal").modal('toggle');
    }
    /* Search and Filters */
    self.searchQueryString = ko.observable('');
    self.categoryQueryString = ko.observable('');
    self.dataReadyForView = ko.observable(true);
    self.notPageSearch = ko.observable(false);
    self.showcaseData = function (data) {
        self.products(ko.utils.arrayMap(data.data, function (product) {
            return new productViewModel(product, self.cartViewModel.removeFromCart, self.cartViewModel.setSessionStorage);
        }));
        self.MergeSession();
        self.totalPages(data.totalPages);
        setTimeout(() => {
            self.dataReadyForView(true);
        }, 300);
    }
    self.searchProduct = function () {
        self.dataReadyForView(false);
        var pageUrl = "/Home/SearchProducts";
        self.categoryQueryString(null);
        self.searchQueryString(self.searchQueryString());
        if (self.notPageSearch() === true) {
            self.pageNumber(1);
        }
        var parameters = { "Query": self.searchQueryString(), "Page": self.pageNumber() };
        $.ajax({
            type: 'GET',
            url: pageUrl,
            traditional: true,
            //async: false,
            data: parameters,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                self.showcaseData(data);
            },
            error: function (data, success, error) {
                alert("Error : " + error);
            }
        });
    }

    self.filterByCategory = function (catStr) {
        self.categoryQueryString(catStr);
        self.searchProductByCategory();
    }

    self.searchProductByCategory = function () {
        self.dataReadyForView(false);
        self.searchQueryString(null);
        if (self.notPageSearch() === true) {
            self.pageNumber(1);
        }
        var pageUrl = "/Home/SearchCategories";
        var parameters = { "Query": self.categoryQueryString(), "Page": self.pageNumber() };
        $.ajax({
            type: 'GET',
            url: pageUrl,
            //async: false,
            traditional: true,
            data: parameters,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                self.showcaseData(data);
            },
            error: function (data, success, error) {
                alert("Error : " + error);
            }
        });
    }

    self.removeFilters = function () {
        self.searchQueryString('');
        self.categoryQueryString('');
        self.searchProduct();
    }
    /* Paging */
    self.pageGroup = ko.observableArray();
    self.totalPages = ko.observable(0);
    self.pageNumber = ko.observable(1);

    self.setPages = ko.computed(function () {
        self.pageGroup.removeAll();
        for (var i = 1; i < self.totalPages() + 1; i++) {
            var pageSelected = false;
            if (self.pageNumber() === i) {
                pageSelected = true;
            }
            self.pageGroup.push({ page: i, isSelected: pageSelected });
        }
    });

    self.goToPage = function (pg) {
        self.pageNumber(pg.page);
        self.notPageSearch(false);
        if (self.categoryQueryString() === null || self.categoryQueryString() === undefined || self.categoryQueryString() === '') {
            self.searchProduct();
        } else {
            self.searchProductByCategory();
        }
        self.pageNumber(pg.page);
        self.notPageSearch(true);
    }
    /* Activate */
    self.activate = function (categories, totalPages) {
        self.categories(categories);
        self.totalPages(totalPages);
        self.MergeSession();
    }
    /* SignalR */
    connection.on("updateProduct", function (pr) {
        console.log('product updated');
        var products = ko.utils.arrayFilter(self.products(),
            function (value) { return value.ID() === pr.id; });

        if (products.length > 0) {
            products[0].title(pr.title);
            products[0].descriptionShort(pr.descriptionShort);
            products[0].descriptionLong(pr.descriptionLong);
            products[0].category(pr.category);
            products[0].price(pr.price);
            products[0].imageSource(pr.imageSource);
            products[0].manufacturer(pr.manufacturer);
        }
    });
    self.showcasedProduct = ko.observable(new productViewModel(null, cartViewModel.removeFromCart, cartViewModel.setSessionStorage));
    connection.on("editedOrCreatedProduct", function (pr) {
        console.log('opening edit/create modal');
        self.showcasedProduct(new productViewModel(pr, cartViewModel.removeFromCart, cartViewModel.setSessionStorage));
        self.alterModalToggle();
        setTimeout(function () { $('#alterModal').modal('hide'); }, 10000);
    });

    self.alterModalToggle = function () {
        $("#alterModal").modal('toggle');
    }
}

var cartViewModel = function (products) {
    var self = this;

    self.setSessionStorage = function () {
        sessionStorage.setItem('myShop', ko.toJSON(self.products));
    }

    self.removeFromCart = function (product) {
        self.products.remove(product);
    }

    self.products = ko.observableArray(ko.utils.arrayMap(products, function (product) {
        return new productViewModel(product, self.removeFromCart, self.setSessionStorage);
    }));

    self.addToCart = function (product) {
        self.products.push(product);
        self.products()[self.products().indexOf(product)].addQuantity();
    }

    self.totalCartPrice = ko.computed(function () {
        var total = 0;
        if (self.products().length > 0) {
            for (var i = 0; i < self.products().length; i++) {
                total = total + self.products()[i].totalPrice();
            }
        }
        return total;
    });
}
