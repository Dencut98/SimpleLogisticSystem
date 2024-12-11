function addToOrder(itemId, itemName, itemPrice, maxQuantity, itemWeight) {
    var quantityInput = document.getElementById('quantity-' + itemId);
    var quantity = parseInt(quantityInput.value);
    itemWeight = parseFloatWithComma(itemWeight);

    if (quantity > 0 && quantity <= maxQuantity) {
        var orderItems = document.getElementById('orderItems');
        var existingItem = orderItems.querySelector('input[name="OrderItems[' + itemId + '].ItemId"]');

        if (existingItem) {
            // Update existing item
            var existingQuantityInput = existingItem.parentElement.querySelector('input[type="number"]');
            var newQuantity = parseInt(existingQuantityInput.value) + quantity;
            if (newQuantity > maxQuantity) {
                alert('Exceeds maximum quantity.');
                return;
            }
            existingQuantityInput.value = newQuantity;
            updateOrderItem(existingQuantityInput, itemPrice, itemWeight);
        } else {
            // Add new item
            var noItemsMessage = document.getElementById('noItemsMessage');

            if (noItemsMessage) {
                noItemsMessage.remove();
            }

            var orderItem = document.createElement('div');
            orderItem.className = 'list-group-item';
            orderItem.innerHTML = '<p><strong>Item Name:</strong> ' + itemName + '</p>' +
                '<p><strong>Quantity:</strong> <input type="number" class="form-control form-control-sm d-inline-block" style="width: 60px;" value="' + quantity + '" min="1" max="' + maxQuantity + '" onchange="updateOrderItem(this, ' + itemPrice + ', ' + itemWeight + ')"></p>' +
                '<p><strong>Price:</strong> <span class="item-price">' + (itemPrice * quantity).toFixed(2) + ' Sek</span></p>' +
                '<p><strong>Weight:</strong> <span class="item-weight">' + (itemWeight * quantity).toFixed(2) + ' Kg</span></p>' +
                '<button type="button" class="btn btn-danger btn-sm" onclick="removeFromOrder(this, ' + (itemWeight * quantity) + ', ' + (itemPrice * quantity) + ', ' + quantity + ')">Remove</button>' +
                '<input type="hidden" name="OrderItems[' + (orderItems.children.length) + '].ItemId" value="' + itemId + '">' +
                '<input type="hidden" name="OrderItems[' + (orderItems.children.length) + '].Quantity" value="' + quantity + '">';

            orderItems.appendChild(orderItem);
            updateTotalWeight(itemWeight * quantity);
            updateTotalQuantity(quantity);
            updateTotalPrice(itemPrice * quantity);
        }
    } else {
        alert('Please enter a valid quantity.');
    }
}

function updateOrderItem(input, itemPrice, itemWeight) {
    var previousQuantity = parseInt(input.defaultValue); // Store the previous quantity
    var quantity = parseInt(input.value);
    var maxQuantity = parseInt(input.max);

    if (quantity > 0 && quantity <= maxQuantity) {
        var itemPriceElement = input.parentElement.nextElementSibling.querySelector('.item-price');
        var itemWeightElement = input.parentElement.nextElementSibling.nextElementSibling.querySelector('.item-weight');
        var previousWeight = parseFloat(itemWeightElement.textContent);
        var previousPrice = parseFloat(itemPriceElement.textContent);

        // Calculate floating-point prices and weights, then update
        var updatedPrice = parseFloat(itemPrice * quantity).toFixed(2);
        var updatedWeight = parseFloat(itemWeight * quantity).toFixed(2);

        // Directly set the text content, including only one unit
        itemPriceElement.textContent = updatedPrice + ' Sek';
        itemWeightElement.textContent = updatedWeight + ' Kg';

        var hiddenInput = input.parentElement.parentElement.querySelector('input[type="hidden"][name^="OrderItems"][name$=".Quantity"]');
        hiddenInput.value = quantity;

        updateTotalWeight((itemWeight * quantity) - previousWeight);
        updateTotalQuantity(quantity - previousQuantity);
        updateTotalPrice((itemPrice * quantity) - previousPrice);

        // Update the default value to the new quantity
        input.defaultValue = quantity;
    } else {
        alert('Please enter a valid quantity.');
        input.value = input.defaultValue;
    }
}

function removeFromOrder(button, itemWeight, itemPrice, quantity) {
    var orderItem = button.parentElement;
    orderItem.remove();

    updateTotalWeight(-itemWeight);
    updateTotalQuantity(-quantity);
    updateTotalPrice(-itemPrice);

    var orderItems = document.getElementById('orderItems');
    if (orderItems.children.length === 0) {
        var noItemsMessage = document.createElement('div');
        noItemsMessage.className = 'list-group-item';
        noItemsMessage.id = 'noItemsMessage';
        noItemsMessage.innerHTML = '<p>No items in the order yet.</p>';
        orderItems.appendChild(noItemsMessage);
    }
}

function updateTotalWeight(weightChange) {
    var totalWeightElement = document.getElementById('totalWeight');
    var currentWeight = parseFloat(totalWeightElement.textContent) || 0;
    totalWeightElement.textContent = (currentWeight + weightChange).toFixed(2); // Ensures float display
}

function updateTotalQuantity(quantityChange) {
    var totalQuantityElement = document.getElementById('totalQuantity');
    var currentQuantity = parseInt(totalQuantityElement.textContent) || 0;
    totalQuantityElement.textContent = currentQuantity + quantityChange;
}

function updateTotalPrice(priceChange) {
    var totalPriceElement = document.getElementById('totalPrice');
    var currentPrice = parseFloat(totalPriceElement.textContent) || 0;
    totalPriceElement.textContent = (currentPrice + priceChange).toFixed(2); // Ensures float display
}

function parseFloatWithComma(string) {
    // Replace comma with dot
    var normalizedString = string.replace(',', '.');
    var result = parseFloat(normalizedString);

    if (!isNaN(result)) {
        return result;
    } else {
        console.log("The string is not a valid float.");
        return null;
    }
}