function downloadExcel() {
    const date = document.getElementById("inputDate").value;
    if (!date) {
        alert("Please select a date for export.");
        return;
    }

    fetch(`/Report/DownloadExcel`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({ date: date }),
    })
        .then(response => response.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.style.display = "none";
            a.href = url;
            a.download = "TicketData.xlsx";
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => console.error("Error downloading Excel:", error));
}
