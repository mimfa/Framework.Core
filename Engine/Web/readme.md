# PointerJS Examples

Welcome to the PointerJS examples page. PointerJS is a powerful tool for interacting with web elements using JavaScript commands executed from a C# environment. Here, we'll guide you through simple and complex examples using PointerJS.

## Example 1: Automating a Login Process
This example demonstrates how to fill in a login form and submit it using PointerJS.
```javascript
// Automating a Login Process
PointerJS pointer = new PointerJS((scripts, args) => yourBrowser.ExecuteScript(scripts, args));

pointer.Select("#username").SetValue("myUsername").Perform();
pointer.Select("#password").SetValue("myPassword").Perform();
pointer.Select("#loginButton").Click().Perform();
```

## Example 2: Extracting Product Information
This example shows how to extract product titles and prices from an e-commerce website.
```javascript
// Extracting Product Information
PointerJS pointer = new PointerJS((scripts, args) => yourBrowser.ExecuteScript(scripts, args));

var productTitles = pointer.Select(".product-title").All().ToArray();
var productPrices = pointer.Select(".product-price").All().ToArray();

for (int i = 0; i < productTitles.Length; i++) {
    Console.WriteLine($"Product: {productTitles[i].GetValue()}, Price: {productPrices[i].GetValue()}");
}
```

## Example 3: Navigating a Multi-page Form
This example automates filling out a multi-page form and navigating through it.
```javascript
// Navigating a Multi-page Form
PointerJS pointer = new PointerJS((scripts, args) => yourBrowser.ExecuteScript(scripts, args));

pointer.Select("input[name='firstName']").SetValue("John").Perform();
pointer.Select("input[name='lastName']").SetValue("Doe").Perform();
pointer.Select("#nextButton").Click().Perform();
```

## Example 4: Extracting Data from a Table
This example shows how to extract data from an HTML table and process it.
```javascript
// Extracting Data from a Table
PointerJS pointer = new PointerJS((scripts, args) => yourBrowser.ExecuteScript(scripts, args));

foreach (var row in pointer.Select("table#data tbody tr").All()) {
    var cells = row.Select("td").All().ToArray();
    Console.WriteLine($"Row Data: {string.Join(", ", cells.Select(cell => cell.GetValue()))}");
}
```

## Example 5: Handling Dynamic Content
This example demonstrates how to interact with dynamic content that requires executing JavaScript.
```javascript
// Handling Dynamic Content
PointerJS pointer = new PointerJS((scripts, args) => yourBrowser.ExecuteScript(scripts, args));

pointer.Execute("document.querySelector('.load-more-button').click()");
var newContent = pointer.Select(".new-content").All().ToArray();
Console.WriteLine($"New Content: {string.Join(", ", newContent.Select(content => content.GetValue()))}");
```
