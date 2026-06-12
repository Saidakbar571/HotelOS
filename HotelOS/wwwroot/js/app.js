/* ═══════════════════════════════════════════════════════
   HotelOS — app.js  (Redesign v2)
   ═══════════════════════════════════════════════════ */

/* ─── 1. API helper ──────────────────────────────────── */

let currentUser = null;

async function apiCall(method, url, body = null) {
  const options = { method, headers: { 'Content-Type': 'application/json' } };
  if (body) options.body = JSON.stringify(body);
  try {
    const res = await fetch(url, options);
    return await res.json();
  } catch {
    return { ok: false, message: 'Server bilan aloqa yo\'q.' };
  }
}
const GET  = (url)       => apiCall('GET',  url);
const POST = (url, body) => apiCall('POST', url, body);
const PUT  = (url)       => apiCall('PUT',  url);

/* ─── 2. UI helper ───────────────────────────────────── */

function showToast(message, type = '') {
  const el = document.getElementById('toast');
  const icon = type === 'success' ? '✓' : type === 'danger' ? '✕' : 'ℹ';
  el.innerHTML = `<span>${icon}</span><span>${message}</span>`;
  el.className = `toast show ${type}`;
  setTimeout(() => el.className = 'toast', 3000);
}

function openModal(title, bodyHtml) {
  document.getElementById('modalTitle').textContent = title;
  document.getElementById('modalBody').innerHTML    = bodyHtml;
  document.getElementById('modal').classList.remove('hidden');
}
function closeModal() { document.getElementById('modal').classList.add('hidden'); }
function onModalOverlayClick(e) { if (e.target.id === 'modal') closeModal(); }

function startClock() {
  const el = document.getElementById('clock');
  const tick = () => { if (el) el.textContent = new Date().toLocaleString('uz-UZ'); };
  tick(); setInterval(tick, 1000);
}

function statusBadge(status) {
  const map = {
    Confirmed:    'badge-success',
    Available:    'badge-success',
    Canceled:     'badge-danger',
    Abandoned:    'badge-gray',
    Requested:    'badge-warning',
    Reserved:     'badge-warning',
    BeingServiced:'badge-info',
    Occupied:     'badge-danger',
    NotAvailable: 'badge-gray',
  };
  const icons = {
    Confirmed:'✓', Available:'○', Canceled:'✕', Abandoned:'–',
    Requested:'⏱', Reserved:'⏱', BeingServiced:'🔧', Occupied:'●', NotAvailable:'✕'
  };
  return `<span class="badge ${map[status]||'badge-gray'}">${icons[status]||''}${status}</span>`;
}

/* ─── 3. Auth ─────────────────────────────────────────── */

async function login() {
  const accountId = document.getElementById('loginId').value.trim();
  const password  = document.getElementById('loginPwd').value.trim();
  const result    = await POST('/api/auth/login', { accountId, password });
  if (!result.ok) {
    const err = document.getElementById('loginError');
    err.textContent = result.message;
    err.classList.remove('hidden');
    return;
  }
  currentUser = { accountId: result.accountId, role: result.role, name: result.name };
  document.getElementById('loginScreen').classList.add('hidden');
  document.getElementById('app').classList.remove('hidden');
  document.getElementById('loginError').classList.add('hidden');
  initApp();
}

function logout() {
  currentUser = null;
  document.getElementById('app').classList.add('hidden');
  document.getElementById('loginScreen').classList.remove('hidden');
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('loginPwd')
    .addEventListener('keydown', e => { if (e.key === 'Enter') login(); });
});

/* ─── 4. Navigation ──────────────────────────────────── */

const NAV_BY_ROLE = {
  receptionist: [
    { icon: '📊', label: 'Dashboard',   page: 'dashboard',  color: '#6366f1' },
    { icon: '🛏️', label: 'Xonalar',     page: 'rooms',      color: '#0891b2' },
    { icon: '📋', label: 'Bronlar',      page: 'bookings',   color: '#7c3aed' },
    { icon: '✅', label: 'Check-in/out', page: 'checkinout', color: '#059669' },
    { icon: '🧹', label: 'Tozalash',     page: 'cleaning',   color: '#d97706' },
    { icon: '🍽️', label: 'Xizmatlar',   page: 'services',   color: '#e11d48' },
    { icon: '👥', label: 'Mehmonlar',    page: 'guests',     color: '#2563eb' },
    { icon: '🏢', label: 'Filiallar',    page: 'branches',   color: '#0f766e' },
  ],
  guest: [
    { icon: '🔍', label: 'Xona qidirish',  page: 'search',     color: '#9333ea' },
    { icon: '📋', label: 'Mening bronim',  page: 'mybookings', color: '#7c3aed' },
    { icon: '🍽️', label: 'Xizmat so\'rash',page: 'services',   color: '#e11d48' },
  ],
  housekeeper: [
    { icon: '🧹', label: 'Tozalash', page: 'cleaning', color: '#d97706' },
    { icon: '🛏️', label: 'Xonalar',  page: 'rooms',    color: '#0891b2' },
  ],
};

const PAGE_TITLES = {
  dashboard:  'Dashboard',    rooms:      'Xonalar',
  bookings:   'Bronlar',      checkinout: 'Check-in / Check-out',
  cleaning:   'Tozalash',     services:   'Xona Xizmatlari',
  guests:     'Mehmonlar',    branches:   'Filiallar',
  search:     'Xona Qidirish',mybookings: 'Mening Bronlarim',
};

const PAGE_COLORS = {
  dashboard: '#6366f1', rooms: '#0891b2', bookings: '#7c3aed',
  checkinout:'#059669', cleaning:'#d97706', services:'#e11d48',
  guests:'#2563eb',     branches:'#0f766e', search:'#9333ea', mybookings:'#7c3aed'
};

const ROLE_META = {
  receptionist: { label: 'Qabulchi',  icon: '💼', color: '#6366f1' },
  guest:         { label: 'Mehmon',   icon: '👤', color: '#7c3aed' },
  housekeeper:   { label: 'Xizmatchi',icon: '🧹', color: '#d97706' },
};

function initApp() {
  const navItems = NAV_BY_ROLE[currentUser.role] || NAV_BY_ROLE.guest;
  const meta     = ROLE_META[currentUser.role] || ROLE_META.guest;
  const initials = currentUser.name.split(' ').map(w=>w[0]).join('').toUpperCase().slice(0,2);

  document.getElementById('userInfo').innerHTML = `
    <div class="user-info-row">
      <div class="user-avatar" style="background:${meta.color}">${initials}</div>
      <div>
        <div class="user-name">${currentUser.name}</div>
        <div class="user-role">${meta.icon} ${meta.label}</div>
      </div>
    </div>`;

  document.getElementById('sidebarNav').innerHTML = navItems.map(item => `
    <div class="nav-item" id="nav-${item.page}" data-page="${item.page}"
         onclick="navigate('${item.page}')" style="--section-color:${item.color}">
      <div class="icon-wrap">${item.icon}</div>
      <span>${item.label}</span>
    </div>`).join('');

  startClock();
  navigate(navItems[0].page);
}

function navigate(page) {
  document.querySelectorAll('.nav-item').forEach(el => el.classList.remove('active'));
  document.getElementById('nav-'+page)?.classList.add('active');
  document.getElementById('pageTitle').textContent = PAGE_TITLES[page] || page;

  const color = PAGE_COLORS[page] || '#6366f1';
  const dot = document.getElementById('pageColorDot');
  if (dot) dot.style.background = color;

  const pages = { dashboard, rooms, bookings, checkinout, cleaning, services, guests, branches, search, mybookings };
  pages[page]?.();
}

/* ─── 5. Sahifalar ───────────────────────────────────── */

/* ── Dashboard ── */
async function dashboard() {
  const d = await GET('/api/dashboard');
  if (!d.ok) return;

  document.getElementById('content').innerHTML = `
    <div class="stats-grid">
      <div class="stat-card indigo">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">🛏️</div>
        <div class="stat-label">Jami xonalar</div>
        <div class="stat-value">${d.totalRooms}</div>
      </div>
      <div class="stat-card cyan">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">✅</div>
        <div class="stat-label">Bo'sh xonalar</div>
        <div class="stat-value">${d.available}</div>
        <div class="stat-sub">Hozir mavjud</div>
      </div>
      <div class="stat-card rose">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">🔑</div>
        <div class="stat-label">Band xonalar</div>
        <div class="stat-value">${d.occupied}</div>
      </div>
      <div class="stat-card amber">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">⏱️</div>
        <div class="stat-label">Hold (cache)</div>
        <div class="stat-value">${d.reserved}</div>
        <div class="stat-sub">10 daqiqa</div>
      </div>
      <div class="stat-card violet">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">📋</div>
        <div class="stat-label">Jami bronlar</div>
        <div class="stat-value">${d.totalBookings}</div>
      </div>
      <div class="stat-card emerald">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">🔒</div>
        <div class="stat-label">Faol holdlar</div>
        <div class="stat-value">${d.activeHolds}</div>
      </div>
      <div class="stat-card amber">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">💰</div>
        <div class="stat-label">Daromad</div>
        <div class="stat-value">$${Number(d.totalRevenue).toFixed(0)}</div>
      </div>
      <div class="stat-card indigo">
        <div class="stat-glow"></div>
        <div class="stat-icon-wrap">🧹</div>
        <div class="stat-label">Tozalash</div>
        <div class="stat-value">${d.pendingTasks}</div>
        <div class="stat-sub">Kutayotgan</div>
      </div>
    </div>

    <div class="flow-card">
      <div style="font-size:14px;font-weight:700;color:#4338ca;margin-bottom:14px">
        📌 Bron oqimi
      </div>
      <div class="flow-step">
        <div class="flow-num">1</div>
        <div class="flow-text">
          <strong>Hold</strong> — Xona 10 daqiqa <em>Reserved</em> (cache) bo'ladi, Bron = Requested
        </div>
      </div>
      <div class="flow-step">
        <div class="flow-num">2</div>
        <div class="flow-text">
          <strong>Pay</strong> — To'lov muvaffaqiyatli → <em>Confirmed</em> | Muvaffaqiyatsiz → <em>Abandoned</em>
        </div>
      </div>
      <div class="flow-step">
        <div class="flow-num" style="background:#d97706">⚙</div>
        <div class="flow-text">
          <strong>Worker</strong> — Har 60 soniyada muddati o'tgan <em>Requested</em> bronlarni <em>Abandoned</em> qiladi
        </div>
      </div>
      <div style="margin-top:12px">
        <a href="/scalar/v1" target="_blank" class="swagger-link">📚 API ni sinab ko'ring →</a>
      </div>
    </div>`;
}

/* ── Rooms ── */
async function rooms() {
  const d = await GET('/api/rooms');
  if (!d.ok) return;

  const styleIcons = { Standard: '🛏️', Deluxe: '✨', FamilySuite: '👨‍👩‍👧', BusinessSuite: '💼' };

  const countByStatus = {};
  d.rooms.forEach(r => { countByStatus[r.status] = (countByStatus[r.status]||0)+1; });

  document.getElementById('content').innerHTML = `
    <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(130px,1fr));gap:10px;margin-bottom:16px">
      ${[
        { s:'Available',   l:'Bo\'sh',      clr:'#10b981', bg:'rgba(16,185,129,.1)' },
        { s:'Occupied',    l:'Band',         clr:'#ef4444', bg:'rgba(239,68,68,.1)' },
        { s:'Reserved',    l:'Kutilmoqda',   clr:'#f59e0b', bg:'rgba(245,158,11,.1)' },
        { s:'BeingServiced',l:'Tozalanmoqda',clr:'#3b82f6', bg:'rgba(59,130,246,.1)' },
      ].map(x => `
        <div style="background:${x.bg};border:1.5px solid ${x.clr}25;border-radius:12px;padding:12px;text-align:center">
          <div style="font-size:22px;font-weight:800;color:${x.clr}">${countByStatus[x.s]||0}</div>
          <div style="font-size:11px;color:var(--muted);font-weight:600;margin-top:2px">${x.l}</div>
        </div>`).join('')}
    </div>

    <div class="card">
      <div class="card-header">
        <h3>🛏️ Barcha Xonalar</h3>
        ${currentUser.role==='receptionist'
          ? `<button class="btn btn-sm" style="background:#0891b2;color:#fff" onclick="openAddRoomModal()">+ Qo'shish</button>`
          : ''}
      </div>
      <div class="card-body">
        <div class="rooms-grid">
          ${d.rooms.map(r => `
            <div class="room-card ${r.status.toLowerCase()}" onclick="openRoomDetail('${r.roomNumber}')">
              <div class="room-style-icon">${styleIcons[r.style]||'🛏️'}</div>
              <div class="room-number">${r.roomNumber}</div>
              <div class="room-style">${r.style}</div>
              <div class="room-price">$${r.pricePerNight}/tun</div>
              <div class="mt-12">${statusBadge(r.status)}</div>
              ${r.isSmoking ? '<div class="muted" style="font-size:11px;margin-top:4px">🚬 Chekish</div>' : ''}
            </div>`).join('')}
        </div>
      </div>
    </div>`;
}

function openAddRoomModal() {
  openModal('Yangi Xona Qo\'shish', `
    <div class="form-group">
      <label>Xona raqami</label>
      <input id="nr_num" placeholder="masalan: 205">
    </div>
    <div class="form-row">
      <div class="form-group">
        <label>Xona turi</label>
        <select id="nr_style">
          <option>Standard</option><option>Deluxe</option>
          <option>FamilySuite</option><option>BusinessSuite</option>
        </select>
      </div>
      <div class="form-group">
        <label>Narx ($/tun)</label>
        <input id="nr_price" type="number" value="100">
      </div>
    </div>
    <div class="form-group">
      <label><input type="checkbox" id="nr_smoke" style="width:auto;margin-right:6px"> Chekish mumkin</label>
    </div>
    <button class="btn btn-full mt-12" style="background:#0891b2;color:#fff" onclick="submitAddRoom()">
      🛏️ Qo'shish
    </button>
    <div id="nr_msg"></div>`);
}

async function submitAddRoom() {
  const result = await POST('/api/rooms', {
    roomNumber: document.getElementById('nr_num').value.trim(),
    style:      document.getElementById('nr_style').value,
    price:      +document.getElementById('nr_price').value,
    isSmoking:  document.getElementById('nr_smoke').checked,
    branchId:   'BRANCH1'
  });
  document.getElementById('nr_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">${result.message}</div>`;
  if (result.ok) setTimeout(() => { closeModal(); rooms(); }, 1200);
}

async function openRoomDetail(roomNumber) {
  const d = await GET('/api/rooms');
  const room = d.rooms.find(r => r.roomNumber === roomNumber);
  if (!room) return;

  openModal(`Xona ${roomNumber}`, `
    <div class="detail-grid">
      <div class="detail-item"><div class="detail-label">Raqam</div><div class="detail-value">${room.roomNumber}</div></div>
      <div class="detail-item"><div class="detail-label">Tur</div><div class="detail-value">${room.style}</div></div>
      <div class="detail-item"><div class="detail-label">Holat</div><div class="detail-value">${statusBadge(room.status)}</div></div>
      <div class="detail-item"><div class="detail-label">Narx</div><div class="detail-value" style="color:#0891b2">$${room.pricePerNight}/tun</div></div>
    </div>
    ${currentUser.role==='receptionist' ? `
    <hr class="divider">
    <div class="form-group mt-12">
      <label>Holatni o'zgartirish</label>
      <select id="room_status">
        <option>Available</option><option>NotAvailable</option><option>BeingServiced</option>
      </select>
    </div>
    <button class="btn btn-full" style="background:#0891b2;color:#fff" onclick="updateRoomStatus('${roomNumber}')">
      Yangilash
    </button>
    <div id="rs_msg"></div>` : ''}`);
}

async function updateRoomStatus(roomNumber) {
  const status = document.getElementById('room_status').value;
  const result = await PUT(`/api/rooms/${roomNumber}/status?status=${status}`);
  document.getElementById('rs_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">${result.message}</div>`;
  if (result.ok) setTimeout(() => { closeModal(); rooms(); }, 1000);
}

/* ── Bookings ── */
async function bookings() {
  const d = await GET('/api/bookings');
  if (!d.ok) return;

  const confirmed = d.bookings.filter(b=>b.status==='Confirmed').length;
  const pending   = d.bookings.filter(b=>b.status==='Requested').length;

  document.getElementById('content').innerHTML = `
    <div style="display:grid;grid-template-columns:repeat(3,1fr);gap:12px;margin-bottom:16px">
      <div style="background:rgba(124,58,237,.08);border:1px solid rgba(124,58,237,.2);border-radius:12px;padding:14px;text-align:center">
        <div style="font-size:24px;font-weight:800;color:#7c3aed">${d.bookings.length}</div>
        <div style="font-size:11px;color:var(--muted);font-weight:600">Jami bronlar</div>
      </div>
      <div style="background:rgba(16,185,129,.08);border:1px solid rgba(16,185,129,.2);border-radius:12px;padding:14px;text-align:center">
        <div style="font-size:24px;font-weight:800;color:#059669">${confirmed}</div>
        <div style="font-size:11px;color:var(--muted);font-weight:600">Tasdiqlangan</div>
      </div>
      <div style="background:rgba(245,158,11,.08);border:1px solid rgba(245,158,11,.2);border-radius:12px;padding:14px;text-align:center">
        <div style="font-size:24px;font-weight:800;color:#d97706">${pending}</div>
        <div style="font-size:11px;color:var(--muted);font-weight:600">Kutilmoqda</div>
      </div>
    </div>

    <div class="card">
      <div class="card-header">
        <h3>📋 Barcha Bronlar</h3>
        <button class="btn btn-sm" style="background:#7c3aed;color:#fff" onclick="openHoldModal()">+ Yangi Bron</button>
      </div>
      <div class="card-body table-wrap">
        <table>
          <thead>
            <tr><th>Bron ID</th><th>Mehmon</th><th>Xona</th><th>Kirish</th><th>Chiqish</th><th>Holat</th><th>Amal</th></tr>
          </thead>
          <tbody>
            ${d.bookings.map(b => `
              <tr>
                <td><span class="booking-id">${b.id}</span></td>
                <td>${b.guestId}</td>
                <td>${b.room||'—'}</td>
                <td>${b.checkIn}</td>
                <td>${b.checkOut}</td>
                <td>${statusBadge(b.status)}</td>
                <td style="display:flex;gap:6px">
                  <button class="btn btn-outline btn-sm" onclick="openBookingDetail('${b.id}')">Ko'rish</button>
                  ${b.status==='Confirmed'
                    ? `<button class="btn btn-danger btn-sm" onclick="cancelBooking('${b.id}')">Bekor</button>`:''}
                </td>
              </tr>`).join('')}
          </tbody>
        </table>
      </div>
    </div>`;
}

function openHoldModal() {
  const today = new Date().toISOString().split('T')[0];
  openModal('Yangi Bron — Qadam 1: Band qilish', `
    <div class="alert alert-info">
      <span>ℹ️</span>
      <div><strong>Qanday ishlaydi:</strong><br>
      1. Xona <strong>10 daqiqa</strong> band qilinadi (Requested)<br>
      2. To'lov → Confirmed | Muddat o'tsa → Abandoned</div>
    </div>
    <div class="form-row mt-12">
      <div class="form-group">
        <label>Mehmon ID</label>
        <input id="h_guest" value="${currentUser.accountId}">
      </div>
      <div class="form-group">
        <label>Xona raqami</label>
        <input id="h_room" placeholder="101">
      </div>
    </div>
    <div class="form-row">
      <div class="form-group">
        <label>Kirish sanasi</label>
        <input id="h_checkin" type="date" value="${today}">
      </div>
      <div class="form-group">
        <label>Kun soni</label>
        <input id="h_days" type="number" value="2" min="1">
      </div>
    </div>
    <div class="form-group">
      <label>Avans miqdori ($)</label>
      <input id="h_advance" type="number" value="100">
    </div>
    <button class="btn btn-full mt-12" style="background:#7c3aed;color:#fff" onclick="submitHold()">
      ⏱️ Xonani 10 daqiqa band qilish
    </button>
    <div id="hold_result"></div>`);
}

async function submitHold() {
  const result = await POST('/api/bookings/hold', {
    guestId:     document.getElementById('h_guest').value.trim(),
    roomNumber:  document.getElementById('h_room').value.trim(),
    checkInDate: document.getElementById('h_checkin').value,
    days:        +document.getElementById('h_days').value,
    advance:     +document.getElementById('h_advance').value,
  });
  if (!result.ok) {
    document.getElementById('hold_result').innerHTML =
      `<div class="alert alert-danger"><span>✕</span>${result.message}</div>`; return;
  }
  showPaymentStep(result.bookingId, result.holdId, +document.getElementById('h_advance').value);
}

function showPaymentStep(bookingId, holdId, amount) {
  document.getElementById('hold_result').innerHTML = `
    <div class="alert alert-success mt-12"><span>✓</span>Xona band qilindi! Bron: <strong>${bookingId}</strong></div>
    <div class="countdown" id="countdown">10:00</div>
    <div class="form-group mt-12">
      <label>To'lov usuli</label>
      <select id="pay_method">
        <option value="cash">Naqd pul</option>
        <option value="credit">Kredit karta</option>
        <option value="check">Chek</option>
      </select>
    </div>
    <div class="form-group">
      <label>Tafsilot (karta uchun ism / bank nomi)</label>
      <input id="pay_detail" placeholder="Ixtiyoriy">
    </div>
    <button class="btn btn-success btn-full" onclick="submitPayment('${bookingId}','${holdId}',${amount})">
      💳 To'lovni tasdiqlash
    </button>
    <div id="pay_result"></div>`;
  startCountdown('countdown', 10*60);
}

function startCountdown(elementId, totalSeconds) {
  const el = document.getElementById(elementId);
  let secs = totalSeconds;
  const timer = setInterval(() => {
    secs--;
    const m = Math.floor(secs/60).toString().padStart(2,'0');
    const s = (secs%60).toString().padStart(2,'0');
    if (!el) { clearInterval(timer); return; }
    el.textContent = `${m}:${s} qoldi`;
    el.className   = secs < 60 ? 'countdown urgent' : 'countdown';
    if (secs <= 0) {
      clearInterval(timer);
      el.textContent = '⏰ Muddat tugadi!';
      showToast('10 daqiqa o\'tdi. Worker bronni Abandoned qildi.', 'danger');
    }
  }, 1000);
}

async function submitPayment(bookingId, holdId, amount) {
  const result = await POST('/api/bookings/pay', {
    bookingId, holdId,
    method: document.getElementById('pay_method').value,
    detail: document.getElementById('pay_detail').value,
    amount,
  });
  document.getElementById('pay_result').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">
      <span>${result.ok?'✓':'✕'}</span>${result.message}
    </div>`;
  if (result.ok) setTimeout(() => { closeModal(); bookings(); }, 1500);
}

async function openBookingDetail(id) {
  const b = await GET(`/api/bookings/${id}`);
  if (!b.ok) return;
  openModal(`Bron: ${b.bookingId}`, `
    <div class="detail-grid">
      <div class="detail-item"><div class="detail-label">ID</div><div class="detail-value booking-id">${b.bookingId}</div></div>
      <div class="detail-item"><div class="detail-label">Mehmon</div><div class="detail-value">${b.guestId}</div></div>
      <div class="detail-item"><div class="detail-label">Xona</div><div class="detail-value">${b.room} (${b.style})</div></div>
      <div class="detail-item"><div class="detail-label">Holat</div><div class="detail-value">${statusBadge(b.status)}</div></div>
      <div class="detail-item"><div class="detail-label">Kirish</div><div class="detail-value">${b.checkIn}</div></div>
      <div class="detail-item"><div class="detail-label">Chiqish</div><div class="detail-value">${b.checkOut}</div></div>
      <div class="detail-item"><div class="detail-label">Avans</div><div class="detail-value">$${b.advanceAmount}</div></div>
      <div class="detail-item"><div class="detail-label">Jami</div><div class="detail-value" style="color:#7c3aed;font-size:16px">${b.total?'$'+b.total.toFixed(2):'—'}</div></div>
    </div>
    ${b.services?.length ? `
      <hr class="divider">
      <div style="font-size:13px;font-weight:700;margin-bottom:8px">Qo'shimcha xizmatlar:</div>
      <ul style="margin:0;padding-left:16px;line-height:2">
        ${b.services.map(s=>`<li style="font-size:13px">${s.type}: ${s.description} — <strong>$${s.cost}</strong></li>`).join('')}
      </ul>`:''}`);
}

async function cancelBooking(id) {
  if (!confirm(`${id} bronini bekor qilasizmi?`)) return;
  const result = await PUT(`/api/bookings/${id}/cancel`);
  showToast(result.message, result.ok?'success':'danger');
  if (result.ok) bookings();
}

/* ── Check-in / Check-out ── */
function checkinout() {
  document.getElementById('content').innerHTML = `
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:20px">
      <div class="card checkin-card">
        <div class="card-header">
          <h3>✅ Check-in</h3>
        </div>
        <div class="card-body">
          <p style="font-size:13px;color:var(--muted);margin-bottom:16px">
            Mehmon kelganda bronni Check-in qiling — xona Occupied bo'ladi.
          </p>
          <div class="form-group">
            <label>Bron ID</label>
            <input id="ci_id" placeholder="RES-XXXXXX">
          </div>
          <button class="btn btn-full" style="background:#059669;color:#fff" onclick="doCheckIn()">
            ✅ Check-in qilish
          </button>
          <div id="ci_msg"></div>
        </div>
      </div>
      <div class="card checkout-card">
        <div class="card-header">
          <h3 style="color:var(--danger)">🚪 Check-out</h3>
        </div>
        <div class="card-body">
          <p style="font-size:13px;color:var(--muted);margin-bottom:16px">
            Mehmon ketganda bronni Check-out qiling — hisob-kitob chiqariladi.
          </p>
          <div class="form-group">
            <label>Bron ID</label>
            <input id="co_id" placeholder="RES-XXXXXX">
          </div>
          <button class="btn btn-danger btn-full" onclick="doCheckOut()">
            🚪 Check-out qilish
          </button>
          <div id="co_msg"></div>
        </div>
      </div>
    </div>`;
}

async function doCheckIn() {
  const id = document.getElementById('ci_id').value.trim();
  const result = await PUT(`/api/bookings/${id}/checkin`);
  document.getElementById('ci_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}"><span>${result.ok?'✓':'✕'}</span>${result.message}</div>`;
}

async function doCheckOut() {
  const id = document.getElementById('co_id').value.trim();
  const result = await PUT(`/api/bookings/${id}/checkout`);
  document.getElementById('co_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">
      <span>${result.ok?'✓':'✕'}</span>
      ${result.message}
      ${result.ok?`<br><strong>Jami: $${result.total?.toFixed(2)}</strong>`:''}
    </div>`;
}

/* ── Cleaning ── */
async function cleaning() {
  const d = await GET('/api/cleaning');
  if (!d.ok) return;

  const pending   = d.tasks.filter(t=>!t.isCompleted).length;
  const completed = d.tasks.filter(t=> t.isCompleted).length;

  document.getElementById('content').innerHTML = `
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px;margin-bottom:16px">
      <div style="background:rgba(217,119,6,.08);border:1px solid rgba(217,119,6,.2);border-radius:12px;padding:16px;text-align:center">
        <div style="font-size:28px;font-weight:800;color:#d97706">${pending}</div>
        <div style="font-size:11px;color:var(--muted);font-weight:600;text-transform:uppercase">Kutayotgan vazifalar</div>
      </div>
      <div style="background:rgba(16,185,129,.08);border:1px solid rgba(16,185,129,.2);border-radius:12px;padding:16px;text-align:center">
        <div style="font-size:28px;font-weight:800;color:#059669">${completed}</div>
        <div style="font-size:11px;color:var(--muted);font-weight:600;text-transform:uppercase">Bajarilgan</div>
      </div>
    </div>

    <div class="card">
      <div class="card-header">
        <h3>🧹 Tozalash Vazifalari</h3>
        <span style="font-size:12px;color:var(--muted)">${d.tasks.length} ta vazifa</span>
      </div>
      <div class="card-body table-wrap">
        <table>
          <thead>
            <tr><th>Xona</th><th>Tavsif</th><th>Boshlanish</th><th>Davom.</th><th>Xizmatchi</th><th>Holat</th><th>Amal</th></tr>
          </thead>
          <tbody>
            ${d.tasks.map(t => `
              <tr>
                <td><strong style="color:#d97706">${t.roomNumber}</strong></td>
                <td>${t.description}</td>
                <td style="font-size:12px;color:var(--muted)">${t.createdAt}</td>
                <td>${t.durationMinutes} daq</td>
                <td>${t.housekeeperName||'—'}</td>
                <td>${statusBadge(t.isCompleted?'Available':'Reserved')}</td>
                <td>
                  ${!t.isCompleted
                    ? `<button class="btn btn-success btn-sm" onclick="completeTask('${t.id}')">✓ Tugallash</button>`
                    : '<span style="color:var(--muted);font-size:12px">✓ Bajarildi</span>'}
                </td>
              </tr>`).join('')}
          </tbody>
        </table>
      </div>
    </div>`;
}

async function completeTask(id) {
  const result = await PUT(`/api/cleaning/${id}/complete`);
  showToast(result.message, result.ok?'success':'danger');
  if (result.ok) cleaning();
}

/* ── Services ── */
function services() {
  document.getElementById('content').innerHTML = `
    <div class="card">
      <div class="card-header">
        <h3>🍽️ Xona Xizmati Qo'shish</h3>
      </div>
      <div class="card-body">
        <div style="display:grid;grid-template-columns:repeat(3,1fr);gap:10px;margin-bottom:20px">
          ${[
            { val:'room',    icon:'🛏️', name:'Xona xizmati' },
            { val:'kitchen', icon:'🍽️', name:'Oshxona' },
            { val:'amenity', icon:'🛁', name:'Qulaylik' },
          ].map(t => `
            <div class="service-type-card" id="st_${t.val}" onclick="selectServiceType('${t.val}')">
              <div class="service-type-icon">${t.icon}</div>
              <div class="service-type-name">${t.name}</div>
            </div>`).join('')}
        </div>
        <input type="hidden" id="sv_type" value="room">

        <div class="form-row">
          <div class="form-group">
            <label>Bron ID</label>
            <input id="sv_booking" placeholder="RES-XXXXXX">
          </div>
          <div class="form-group">
            <label>Narx ($)</label>
            <input id="sv_cost" type="number" value="15">
          </div>
        </div>
        <div class="form-group">
          <label>Tavsif</label>
          <input id="sv_desc" placeholder="masalan: 2 qahva, ertalabki ovqat">
        </div>
        <button class="btn" style="background:#e11d48;color:#fff" onclick="submitService()">
          + Xizmat qo'shish
        </button>
        <div id="sv_msg"></div>
      </div>
    </div>`;

  // Default active
  document.getElementById('st_room')?.classList.add('sel');
}

function selectServiceType(val) {
  document.querySelectorAll('.service-type-card').forEach(c=>c.classList.remove('sel'));
  document.getElementById('st_'+val)?.classList.add('sel');
  document.getElementById('sv_type').value = val;
}

async function submitService() {
  const result = await POST('/api/hotelservices', {
    bookingId:   document.getElementById('sv_booking').value.trim(),
    type:        document.getElementById('sv_type').value,
    description: document.getElementById('sv_desc').value.trim(),
    cost:        +document.getElementById('sv_cost').value,
  });
  document.getElementById('sv_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">
      <span>${result.ok?'✓':'✕'}</span>${result.message}
    </div>`;
}

/* ── Guests ── */
async function guests() {
  const d = await GET('/api/guests');
  if (!d.ok) return;

  document.getElementById('content').innerHTML = `
    <div class="card">
      <div class="card-header">
        <h3>👥 Mehmonlar <span style="font-size:12px;font-weight:400;color:var(--muted);margin-left:8px">${d.guests.length} ta</span></h3>
        <button class="btn btn-sm" style="background:#2563eb;color:#fff" onclick="openRegisterModal()">+ Yangi Mehmon</button>
      </div>
      <div class="card-body table-wrap">
        <table>
          <thead><tr><th></th><th>ID</th><th>Ism</th><th>Email</th><th>Telefon</th><th>Holat</th></tr></thead>
          <tbody>
            ${d.guests.map(g => {
              const initials = g.name.split(' ').map(w=>w[0]).join('').toUpperCase().slice(0,2);
              return `
              <tr>
                <td style="padding:8px 14px">
                  <div class="guest-avatar" style="font-size:11px">${initials}</div>
                </td>
                <td><strong style="color:#2563eb;font-family:monospace;font-size:12px">${g.id}</strong></td>
                <td><strong>${g.name}</strong></td>
                <td style="color:var(--muted)">${g.email}</td>
                <td style="color:var(--muted)">${g.phone}</td>
                <td>${statusBadge(g.status)}</td>
              </tr>`;
            }).join('')}
          </tbody>
        </table>
      </div>
    </div>`;
}

function openRegisterModal() {
  openModal('Yangi Mehmon Ro\'yxati', `
    <div class="form-group"><label>Ism Familya</label><input id="rg_name" placeholder="Ism Familya"></div>
    <div class="form-group"><label>Email</label><input id="rg_email" type="email" placeholder="email@mail.com"></div>
    <div class="form-group"><label>Telefon</label><input id="rg_phone" placeholder="+998901234567"></div>
    <div class="form-group"><label>Parol</label><input id="rg_pwd" type="password"></div>
    <button class="btn btn-full mt-12" style="background:#2563eb;color:#fff" onclick="submitRegister()">
      👤 Ro'yxatdan o'tkazish
    </button>
    <div id="rg_msg"></div>`);
}

async function submitRegister() {
  const result = await POST('/api/auth/register', {
    name:     document.getElementById('rg_name').value.trim(),
    email:    document.getElementById('rg_email').value.trim(),
    phone:    document.getElementById('rg_phone').value.trim(),
    password: document.getElementById('rg_pwd').value,
  });
  document.getElementById('rg_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">
      <span>${result.ok?'✓':'✕'}</span>
      ${result.message}
      ${result.ok?` | ID: <strong>${result.accountId}</strong>`:''}
    </div>`;
  if (result.ok) setTimeout(() => { closeModal(); guests(); }, 1500);
}

/* ── Branches ── */
async function branches() {
  const d = await GET('/api/branches');
  if (!d.ok) return;

  document.getElementById('content').innerHTML = `
    <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(240px,1fr));gap:16px">
      ${d.branches.map(b => `
        <div class="branch-card">
          <div class="branch-card-header">
            <div class="branch-card-icon">🏢</div>
            <div class="branch-card-name">${b.name}</div>
            <div class="branch-card-city">📍 ${b.city}</div>
          </div>
          <div class="branch-card-body">
            <span class="badge badge-info">🛏️ ${b.roomCount} xona</span>
            <span class="badge badge-success">✅ ${b.available} bo'sh</span>
          </div>
        </div>`).join('')}
    </div>`;
}

/* ── Search (guest) ── */
function search() {
  const today = new Date().toISOString().split('T')[0];
  document.getElementById('content').innerHTML = `
    <div class="card" style="border-top:4px solid #9333ea">
      <div class="card-header"><h3>🔍 Xona Qidirish</h3></div>
      <div class="card-body">
        <div style="display:flex;gap:10px;flex-wrap:wrap;align-items:flex-end;margin-bottom:20px">
          <div class="form-group" style="margin:0;min-width:140px">
            <label>Xona turi</label>
            <select id="sr_style">
              <option>Standard</option><option>Deluxe</option>
              <option>FamilySuite</option><option>BusinessSuite</option>
            </select>
          </div>
          <div class="form-group" style="margin:0;min-width:140px">
            <label>Kirish sanasi</label>
            <input id="sr_date" type="date" value="${today}">
          </div>
          <div class="form-group" style="margin:0;min-width:90px">
            <label>Kun soni</label>
            <input id="sr_days" type="number" value="2" min="1">
          </div>
          <button class="btn" style="background:#9333ea;color:#fff;align-self:flex-end" onclick="doSearch()">
            🔍 Qidirish
          </button>
        </div>
        <div id="sr_results"></div>
      </div>
    </div>`;
}

async function doSearch() {
  const style = document.getElementById('sr_style').value;
  const date  = document.getElementById('sr_date').value;
  const days  = document.getElementById('sr_days').value;
  const d     = await GET(`/api/rooms/search?style=${style}&checkIn=${date}&days=${days}`);
  const el    = document.getElementById('sr_results');

  if (!d.ok || !d.rooms.length) {
    el.innerHTML = '<div class="alert alert-info"><span>ℹ️</span>Mos xona topilmadi.</div>'; return;
  }

  el.innerHTML = `
    <div style="font-size:13px;color:var(--muted);margin-bottom:12px">${d.rooms.length} ta xona topildi</div>
    <div class="rooms-grid">
      ${d.rooms.map(r => `
        <div class="room-card available" onclick="openGuestHoldModal('${r.roomNumber}','${date}',${days})">
          <div class="room-number">${r.roomNumber}</div>
          <div class="room-style">${r.style}</div>
          <div class="room-price">$${r.pricePerNight}/tun</div>
          <div class="mt-12">
            <span class="badge badge-success">Bron qilish</span>
          </div>
        </div>`).join('')}
    </div>`;
}

function openGuestHoldModal(roomNumber, date, days) {
  openModal(`Xona ${roomNumber} — Bron qilish`, `
    <div class="alert alert-info"><span>ℹ️</span>Bron yaratilganda xona <strong>10 daqiqa</strong> band qilinadi.</div>
    <div class="form-group mt-12">
      <label>Avans ($)</label>
      <input id="gh_advance" type="number" value="100">
    </div>
    <div class="form-group">
      <label>To'lov usuli</label>
      <select id="gh_method"><option value="cash">Naqd</option><option value="credit">Karta</option></select>
    </div>
    <button class="btn btn-full mt-12" style="background:#9333ea;color:#fff"
      onclick="submitGuestHold('${roomNumber}','${date}',${days})">
      ⏱️ Band qilish
    </button>
    <div id="gh_msg"></div>`);
}

async function submitGuestHold(roomNumber, date, days) {
  const advance = +document.getElementById('gh_advance').value;
  const hold = await POST('/api/bookings/hold', {
    guestId: currentUser.accountId, roomNumber,
    checkInDate: date, days: +days, advance,
  });
  if (!hold.ok) {
    document.getElementById('gh_msg').innerHTML =
      `<div class="alert alert-danger"><span>✕</span>${hold.message}</div>`; return;
  }
  document.getElementById('gh_msg').innerHTML = `
    <div class="alert alert-success"><span>✓</span>Band qilindi! Bron: <strong>${hold.bookingId}</strong></div>
    <div class="countdown" id="gh_countdown">10:00</div>
    <button class="btn btn-success btn-full mt-12"
      onclick="submitGuestPay('${hold.bookingId}','${hold.holdId}',${advance})">
      💳 To'lovni tasdiqlash
    </button>
    <div id="gh_pay_msg"></div>`;
  startCountdown('gh_countdown', 10*60);
}

async function submitGuestPay(bookingId, holdId, amount) {
  const result = await POST('/api/bookings/pay', { bookingId, holdId, method:'cash', detail:'', amount });
  document.getElementById('gh_pay_msg').innerHTML =
    `<div class="alert ${result.ok?'alert-success':'alert-danger'}">
      <span>${result.ok?'✓':'✕'}</span>${result.message}
    </div>`;
  if (result.ok) setTimeout(() => { closeModal(); mybookings(); }, 1500);
}

/* ── Mening bronlarim (guest) ── */
async function mybookings() {
  const d = await GET('/api/bookings');
  if (!d.ok) return;
  const mine = d.bookings.filter(b => b.guestId === currentUser.accountId);

  document.getElementById('content').innerHTML = `
    <div class="card" style="border-top:4px solid #7c3aed">
      <div class="card-header">
        <h3>📋 Mening Bronlarim</h3>
        <span style="font-size:12px;color:var(--muted)">${mine.length} ta bron</span>
      </div>
      <div class="card-body table-wrap">
        ${mine.length === 0
          ? `<div class="alert alert-info">
              <span>ℹ️</span>
              Hali bron yo'q. <a onclick="navigate('search')" style="color:#7c3aed;cursor:pointer;text-decoration:underline">Xona qidirish</a> orqali bron qiling.
            </div>`
          : `<table>
              <thead><tr><th>Bron ID</th><th>Xona</th><th>Kirish</th><th>Chiqish</th><th>Holat</th><th>Amal</th></tr></thead>
              <tbody>
                ${mine.map(b => `
                  <tr>
                    <td><span class="booking-id">${b.id}</span></td>
                    <td>${b.room||'—'}</td>
                    <td>${b.checkIn}</td>
                    <td>${b.checkOut}</td>
                    <td>${statusBadge(b.status)}</td>
                    <td>
                      ${b.status==='Confirmed'
                        ? `<button class="btn btn-danger btn-sm" onclick="cancelBooking('${b.id}')">Bekor qilish</button>`
                        : '—'}
                    </td>
                  </tr>`).join('')}
              </tbody>
            </table>`}
      </div>
    </div>`;
}
