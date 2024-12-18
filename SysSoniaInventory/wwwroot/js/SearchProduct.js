
    const searchInput = document.getElementById("searchInput");
    const suggestions = document.getElementById("suggestions");

    searchInput.addEventListener("input", function () {
        const term = searchInput.value.trim();

        if (term.length > 0) {
        fetch(`/Product/SearchProducts?term=${encodeURIComponent(term)}`)
            .then(response => response.json())
            .then(data => {
                suggestions.innerHTML = "";

                if (data.results.length > 0) {
                    suggestions.style.display = "block";
                    data.results.forEach(product => {
                        const li = document.createElement("li");
                        li.classList.add("dropdown-item");
                        li.textContent = product.name;
                        li.dataset.id = product.id;

                        li.addEventListener("click", function () {
                            window.location.href = `/Product/Details/${product.id}`;
                        });

                        suggestions.appendChild(li);
                    });
                } else {
                    suggestions.style.display = "none";
                }
            });
        } else {
        suggestions.style.display = "none";
        }
    });

    document.addEventListener("click", function (e) {
        if (!suggestions.contains(e.target) && e.target !== searchInput) {
        suggestions.style.display = "none";
        }
    });
