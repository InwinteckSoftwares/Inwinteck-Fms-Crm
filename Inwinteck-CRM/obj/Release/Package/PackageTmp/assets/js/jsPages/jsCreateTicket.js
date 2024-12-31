function filterOptions() {
    // Get the input field and its value
    const input = document.getElementById('jobDescriptionSelect');
    const filter = input.value.toLowerCase();

    // Get the dropdown container and its items
    const dropdown = document.getElementById('jobDescriptionDropdown');
    const items = dropdown.getElementsByClassName('dropdown-item');

    // Show the dropdown
    dropdown.style.display = 'block';

    let anyVisible = false;
    // Loop through all dropdown items to filter based on input
    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        const text = item.textContent || item.innerText;
        // Check if the item text contains the filter string
        if (text.toLowerCase().indexOf(filter) > -1) {
            item.style.display = ''; // Show item
            anyVisible = true;
        } else {
            item.style.display = 'none'; // Hide item
        }
    }

    // If no items are visible, hide the dropdown
    if (!anyVisible) {
        dropdown.style.display = 'none';
    }
}

// Function to select an option from the dropdown
function selectOption(value) {
    // Set the input field value to the selected option
    document.getElementById('jobDescriptionSelect').value = value;
    // Hide the dropdown
    document.getElementById('jobDescriptionDropdown').style.display = 'none';
}

// Event listener to close the dropdown if clicked outside
document.addEventListener('click', function (event) {
    // Check if the click was inside the input field or dropdown
    const isClickInside = document.getElementById('jobDescriptionSelect').contains(event.target) ||
        document.getElementById('jobDescriptionDropdown').contains(event.target);
    // If the click was outside, hide the dropdown
    if (!isClickInside) {
        document.getElementById('jobDescriptionDropdown').style.display = 'none';
    }
});

