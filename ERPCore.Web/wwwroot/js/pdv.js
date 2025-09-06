class PDV {
    constructor() {
        this.saleItems = [];
        this.currentSale = {
            clientName: '',
            clientDocument: '',
            paymentMethod: 'CASH',
            discount: 0,
            totalAmount: 0,
            finalAmount: 0
        };

        this.init();
    }

    init() {
        this.bindEvents();
        this.loadDailySales();
        this.setupScanner();
    }

    bindEvents() {
        // Scanner de produtos
        $('#productCode').on('keypress', (e) => {
            if (e.which === 13) {
                this.scanProduct();
            }
        });

        $('#scanButton').on('click', () => this.scanProduct());

        // Quantidade e desconto
        $(document).on('click', '.btn-quantity', (e) => {
            const button = $(e.target);
            const productId = button.data('product-id');
            const change = button.data('change');
            this.updateQuantity(productId, change);
        });

        $(document).on('click', '.btn-remove', (e) => {
            const productId = $(e.target).data('product-id');
            this.removeItem(productId);
        });

        // Desconto
        $('#discount').on('change', () => this.updateDiscount());

        // Forma de pagamento
        $('#paymentMethod').on('change', () => this.togglePaymentFields());

        // Valor recebido (para dinheiro)
        $('#amountReceived').on('change', () => this.calculateChange());

        // Finalizar venda
        $('#finalizeSale').on('click', () => this.finalizeSale());

        // Imprimir comprovante
        $('#printReceipt').on('click', () => this.printReceipt());
    }

    async scanProduct() {
        const code = $('#productCode').val().trim();
        if (!code) return;

        try {
            const response = await fetch(`/PDV/GetProductByCode?code=${encodeURIComponent(code)}`);
            if (!response.ok) {
                throw new Error('Produto não encontrado');
            }

            const product = await response.json();
            this.addProductToSale(product);
            $('#productCode').val('').focus();
        } catch (error) {
            this.showAlert('error', error.message);
        }
    }

    addProductToSale(product) {
        const existingItem = this.saleItems.find(item => item.productId === product.id);
        
        if (existingItem) {
            existingItem.quantity += 1;
            existingItem.totalPrice = existingItem.unitPrice * existingItem.quantity;
        } else {
            this.saleItems.push({
                productId: product.id,
                productCode: product.code,
                productName: product.name,
                unitPrice: product.priceRetail,
                quantity: 1,
                totalPrice: product.priceRetail
            });
        }

        this.updateSaleTable();
        this.calculateTotals();
    }

    updateQuantity(productId, change) {
        const item = this.saleItems.find(item => item.productId === productId);
        if (item) {
            item.quantity += change;
            if (item.quantity <= 0) {
                this.removeItem(productId);
                return;
            }
            item.totalPrice = item.unitPrice * item.quantity;
            this.updateSaleTable();
            this.calculateTotals();
        }
    }

    removeItem(productId) {
        this.saleItems = this.saleItems.filter(item => item.productId !== productId);
        this.updateSaleTable();
        this.calculateTotals();
    }

    updateSaleTable() {
        const tbody = $('#saleItemsTable tbody');
        tbody.empty();

        this.saleItems.forEach(item => {
            const row = `
                <tr>
                    <td>${item.productCode}</td>
                    <td>${item.productName}</td>
                    <td>R$ ${item.unitPrice.toFixed(2)}</td>
                    <td>
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-secondary btn-quantity" 
                                    data-product-id="${item.productId}" data-change="-1">-</button>
                            <span class="btn btn-outline-light">${item.quantity}</span>
                            <button class="btn btn-outline-secondary btn-quantity" 
                                    data-product-id="${item.productId}" data-change="1">+</button>
                        </div>
                    </td>
                    <td>R$ ${item.totalPrice.toFixed(2)}</td>
                    <td>
                        <button class="btn btn-danger btn-sm btn-remove" 
                                data-product-id="${item.productId}">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    calculateTotals() {
        const subtotal = this.saleItems.reduce((sum, item) => sum + item.totalPrice, 0);
        const discount = parseFloat($('#discount').val()) || 0;
        const total = subtotal - discount;

        $('#subtotal').text(`R$ ${subtotal.toFixed(2)}`);
        $('#total').text(`R$ ${total.toFixed(2)}`);

        this.currentSale.totalAmount = subtotal;
        this.currentSale.discount = discount;
        this.currentSale.finalAmount = total;

        this.calculateChange();
    }

    updateDiscount() {
        this.calculateTotals();
    }

    togglePaymentFields() {
        const method = $('#paymentMethod').val();
        const cashSection = $('#cashSection');
        
        if (method === 'CASH') {
            cashSection.show();
        } else {
            cashSection.hide();
        }
    }

    calculateChange() {
        if ($('#paymentMethod').val() === 'CASH') {
            const amountReceived = parseFloat($('#amountReceived').val()) || 0;
            const change = amountReceived - this.currentSale.finalAmount;
            $('#changeAmount').text(`R$ ${change.toFixed(2)}`);
        }
    }

    async finalizeSale() {
        if (this.saleItems.length === 0) {
            this.showAlert('warning', 'Adicione pelo menos um produto à venda');
            return;
        }

        if (this.currentSale.finalAmount <= 0) {
            this.showAlert('warning', 'O valor total deve ser maior que zero');
            return;
        }

        const saleData = {
            clientName: $('#clientName').val(),
            clientDocument: $('#clientDocument').val(),
            paymentMethod: $('#paymentMethod').val(),
            discount: this.currentSale.discount,
            totalAmount: this.currentSale.totalAmount,
            finalAmount: this.currentSale.finalAmount,
            saleItems: this.saleItems.map(item => ({
                productId: item.productId,
                productCode: item.productCode,
                productName: item.productName,
                unitPrice: item.unitPrice,
                quantity: item.quantity,
                totalPrice: item.totalPrice
            }))
        };

        try {
            const response = await fetch('/PDV/ProcessSale', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(saleData)
            });

            const result = await response.json();

            if (result.success) {
                this.showSuccessModal(result.saleCode, this.currentSale.finalAmount);
                this.resetSale();
            } else {
                throw new Error(result.message);
            }
        } catch (error) {
            this.showAlert('error', error.message);
        }
    }

    showSuccessModal(saleCode, total) {
        $('#saleCodeResult').text(saleCode);
        $('#saleTotalResult').text(`R$ ${total.toFixed(2)}`);
        $('#finishSaleModal').modal('show');
    }

    resetSale() {
        this.saleItems = [];
        this.currentSale = {
            clientName: '',
            clientDocument: '',
            paymentMethod: 'CASH',
            discount: 0,
            totalAmount: 0,
            finalAmount: 0
        };

        this.updateSaleTable();
        this.calculateTotals();
        $('#clientName').val('');
        $('#clientDocument').val('');
        $('#paymentMethod').val('CASH');
        $('#discount').val('0.00');
        $('#amountReceived').val('0.00');
        this.togglePaymentFields();
    }

    async loadDailySales() {
        try {
            const response = await fetch('/PDV/GetDailySales');
            if (response.ok) {
                const data = await response.json();
                $('#dailyTotal').text(`R$ ${data.dailyTotal.toFixed(2)}`);
            }
        } catch (error) {
            console.error('Erro ao carregar vendas diárias:', error);
        }
    }

    setupScanner() {
        // Simulação de scanner de código de barras (pode ser integrado com API real)
        let barcode = '';
        let lastKeyTime = Date.now();

        $(document).on('keypress', (e) => {
            if ($('#productCode').is(':focus')) return;

            const currentTime = Date.now();
            if (currentTime - lastKeyTime > 100) {
                barcode = '';
            }

            if (e.which >= 48 && e.which <= 57) { // Apenas números
                barcode += String.fromCharCode(e.which);
                
                if (barcode.length >= 8) { // Código de barras típico tem 8+ dígitos
                    $('#productCode').val(barcode).focus();
                    this.scanProduct();
                    barcode = '';
                }
            }

            lastKeyTime = currentTime;
        });
    }

    printReceipt() {
        // Implementação básica de impressão
        const printContent = `
            <div style="font-family: monospace; text-align: center;">
                <h3>Comprovante de Venda</h3>
                <p>Código: ${$('#saleCodeResult').text()}</p>
                <p>Data: ${new Date().toLocaleString()}</p>
                <hr>
                ${this.saleItems.map(item => `
                    <div style="text-align: left;">
                        ${item.productCode} - ${item.productName}
                        <div style="float: right;">
                            ${item.quantity} x R$ ${item.unitPrice.toFixed(2)} = R$ ${item.totalPrice.toFixed(2)}
                        </div>
                    </div>
                `).join('')}
                <hr>
                <div style="text-align: left;">
                    <strong>TOTAL: R$ ${this.currentSale.finalAmount.toFixed(2)}</strong>
                </div>
            </div>
        `;

        const printWindow = window.open('', '_blank');
        printWindow.document.write(printContent);
        printWindow.document.close();
        printWindow.print();
    }

    showAlert(type, message) {
        // Implementar sistema de alertas (Toastr ou similar)
        alert(`${type.toUpperCase()}: ${message}`);
    }
}

// Inicializar o PDV quando o documento estiver pronto
$(document).ready(function() {
    window.pdv = new PDV();
});