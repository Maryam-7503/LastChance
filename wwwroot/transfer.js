function toggleSidebar() {
    document.getElementById('sidebar').classList.toggle('open');
  }

  function toggleTheme() {
    document.body.classList.toggle('dark-theme');
    const light = !document.body.classList.contains('dark-theme');
    document.getElementById('lightIcon').classList.toggle('active', light);
    document.getElementById('darkIcon').classList.toggle('active', !light);
  }

  function selectBeneficiary(acc) {
    document.getElementById('toAccount').value = acc;
    updateSummary();
  }

  function updateSummary() {
    const from   = document.getElementById('fromAccount').value;
    const to     = document.getElementById('toAccount').value;
    const amount = parseFloat(document.getElementById('amount').value) || 0;
    const fee    = amount > 0 ? 5.00 : 0;
    const total  = amount + fee;

    document.getElementById('sumFrom').textContent   = from || '—';
    document.getElementById('sumTo').textContent     = to   || '—';
    document.getElementById('sumAmount').textContent = `EGP ${amount.toFixed(2)}`;
    document.getElementById('sumFee').textContent    = `EGP ${fee.toFixed(2)}`;
    document.getElementById('sumTotal').textContent  = `EGP ${total.toFixed(2)}`;
  }

  function transferMoney() {
    const fromAccount = document.getElementById('fromAccount').value;
    const toAccount   = document.getElementById('toAccount').value;
    const amount      = document.getElementById('amount').value;

    ['accountError','toError','amountError'].forEach(id => {
      document.getElementById(id).innerHTML = '';
    });
    document.getElementById('successMessage').classList.remove('show');

    let valid = true;

    if (!fromAccount) {
      document.getElementById('accountError').innerHTML =
        '<span class="material-icons-sharp">error_outline</span> Please select an account';
      valid = false;
    }
    if (toAccount.trim().length < 6) {
      document.getElementById('toError').innerHTML =
        '<span class="material-icons-sharp">error_outline</span> Enter a valid account number (min 6 digits)';
      valid = false;
    }
    if (!amount || parseFloat(amount) <= 0) {
      document.getElementById('amountError').innerHTML =
        '<span class="material-icons-sharp">error_outline</span> Enter a valid amount';
      valid = false;
    }

    if (valid) {
      const amt = parseFloat(amount).toFixed(2);
      document.getElementById('successDetail').textContent =
        `EGP ${amt} sent to account ****${toAccount.slice(-4)} successfully.`;
      document.getElementById('successMessage').classList.add('show');
      document.getElementById('successMessage').scrollIntoView({ behavior:'smooth', block:'nearest' });
    }
  }