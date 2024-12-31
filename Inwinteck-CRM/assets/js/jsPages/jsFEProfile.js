// Function to show ticket numbers in a modal
function showTicketNumbers(type) {
    let ticketNumbers = []; // Initialize an empty array to hold the ticket numbers

    // Determine which ticket numbers to fetch based on the type
    if (type === 'Received') {
        // Parse the JSON string from the hidden input field for received tickets
        ticketNumbers = JSON.parse(document.getElementById('ticketNoOfReceived').value);
    } else if (type === 'Executed') {
        // Parse the JSON string from the hidden input field for executed tickets
        ticketNumbers = JSON.parse(document.getElementById('ticketExecutedNos').value);
    }

    const modalBody = document.getElementById('ticketNumbersModalBody'); // Get the modal body element
    modalBody.innerHTML = ''; // Clear any existing content in the modal body

    if (ticketNumbers.length > 0) {
        const list = document.createElement('ul'); // Create a new unordered list element
        // Iterate through the ticket numbers and create a list item for each
        ticketNumbers.forEach(function (ticketNo) {
            const listItem = document.createElement('li'); // Create a new list item element
            listItem.textContent = ticketNo; // Set the text content of the list item to the ticket number
            list.appendChild(listItem); // Append the list item to the unordered list
        });
        modalBody.appendChild(list); // Append the unordered list to the modal body
    } else {
        modalBody.textContent = 'No ticket numbers available.'; // Display a message if there are no ticket numbers
    }
    $('#ticketNumbersModal').modal('show'); // Show the modal using jQuery
}
