const DB_KEY = 'react-admin-mock-db';

const seedDatabase = {
  users: [
    {
      id: 1,
      name: 'Admin User',
      email: 'admin@example.com',
      password: 'Admin@123',
      role: 'Administrator',
      status: 'Active',
      createdAt: '2026-03-10T09:00:00.000Z',
    },
    {
      id: 2,
      name: 'Maya Brooks',
      email: 'maya@example.com',
      password: 'Maya@123',
      role: 'Manager',
      status: 'Active',
      createdAt: '2026-03-11T10:30:00.000Z',
    },
    {
      id: 3,
      name: 'Dev Patel',
      email: 'dev@example.com',
      password: 'Dev@123',
      role: 'Analyst',
      status: 'Invited',
      createdAt: '2026-03-12T14:15:00.000Z',
    },
  ],
  inventory: [
    {
      id: 101,
      itemName: 'Ultra Laptop',
      sku: 'LP-UL-1001',
      quantity: 18,
      category: 'Electronics',
      price: 1299,
      addedAt: '2026-03-12T12:00:00.000Z',
    },
    {
      id: 102,
      itemName: 'Warehouse Chair',
      sku: 'FR-CH-2040',
      quantity: 44,
      category: 'Furniture',
      price: 149,
      addedAt: '2026-03-13T08:45:00.000Z',
    },
    {
      id: 103,
      itemName: 'Barcode Scanner',
      sku: 'EQ-SC-9005',
      quantity: 26,
      category: 'Equipment',
      price: 249,
      addedAt: '2026-03-14T16:20:00.000Z',
    },
  ],
};

function delay(ms = 400) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}

function ensureDb() {
  const stored = localStorage.getItem(DB_KEY);
  if (!stored) {
    localStorage.setItem(DB_KEY, JSON.stringify(seedDatabase));
    return structuredClone(seedDatabase);
  }

  return JSON.parse(stored);
}

function saveDb(db) {
  localStorage.setItem(DB_KEY, JSON.stringify(db));
}

function nextId(collection) {
  return collection.reduce((highest, item) => Math.max(highest, item.id), 0) + 1;
}

function publicUserShape(user) {
  const { password: _password, ...safeUser } = user;
  return safeUser;
}

function sessionShape(user) {
  return {
    id: user.id,
    name: user.name,
    email: user.email,
    role: user.role,
  };
}

export async function registerUser(payload) {
  await delay();

  const db = ensureDb();
  const emailExists = db.users.some(
    (user) => user.email.toLowerCase() === payload.email.toLowerCase(),
  );

  if (emailExists) {
    throw new Error('An account with this email already exists.');
  }

  const newUser = {
    id: nextId(db.users),
    name: payload.name,
    email: payload.email,
    password: payload.password,
    role: payload.role || 'Administrator',
    status: 'Active',
    createdAt: new Date().toISOString(),
  };

  db.users.unshift(newUser);
  saveDb(db);
  return sessionShape(newUser);
}

export async function loginUser({ email, password }) {
  await delay();

  const db = ensureDb();
  const matchedUser = db.users.find(
    (user) =>
      user.email.toLowerCase() === email.toLowerCase() && user.password === password,
  );

  if (!matchedUser) {
    throw new Error('Invalid email or password.');
  }

  return sessionShape(matchedUser);
}

export async function getDashboardData() {
  await delay();

  const db = ensureDb();
  const activeUsers = db.users.filter((user) => user.status === 'Active').length;
  const totalInventory = db.inventory.reduce((sum, item) => sum + Number(item.quantity), 0);
  const inventoryValue = db.inventory.reduce(
    (sum, item) => sum + Number(item.quantity) * Number(item.price),
    0,
  );

  return {
    stats: [
      { label: 'Total Users', value: db.users.length, accent: 'sunrise' },
      { label: 'Active Users', value: activeUsers, accent: 'ocean' },
      { label: 'Inventory Units', value: totalInventory, accent: 'forest' },
      {
        label: 'Inventory Value',
        value: `$${inventoryValue.toLocaleString()}`,
        accent: 'lava',
      },
    ],
    recentUsers: db.users.slice(0, 4).map(publicUserShape),
    recentInventory: db.inventory.slice(0, 4),
  };
}

export async function getUsers() {
  await delay();
  const db = ensureDb();
  return db.users.map(publicUserShape);
}

export async function addUser(payload) {
  await delay();

  const db = ensureDb();
  const emailExists = db.users.some(
    (user) => user.email.toLowerCase() === payload.email.toLowerCase(),
  );

  if (emailExists) {
    throw new Error('User email already exists.');
  }

  const newUser = {
    id: nextId(db.users),
    name: payload.name,
    email: payload.email,
    password: payload.password || 'Temp@123',
    role: payload.role,
    status: payload.status,
    createdAt: new Date().toISOString(),
  };

  db.users.unshift(newUser);
  saveDb(db);
  return publicUserShape(newUser);
}

export async function deleteUser(userId) {
  await delay(250);

  const db = ensureDb();
  db.users = db.users.filter((user) => user.id !== userId);
  saveDb(db);
}

export async function getInventory() {
  await delay();
  const db = ensureDb();
  return db.inventory;
}

export async function addInventoryItem(payload) {
  await delay();

  const db = ensureDb();
  const skuExists = db.inventory.some(
    (item) => item.sku.toLowerCase() === payload.sku.toLowerCase(),
  );

  if (skuExists) {
    throw new Error('Inventory SKU already exists.');
  }

  const newItem = {
    id: nextId(db.inventory),
    itemName: payload.itemName,
    sku: payload.sku,
    quantity: Number(payload.quantity),
    category: payload.category,
    price: Number(payload.price),
    addedAt: new Date().toISOString(),
  };

  db.inventory.unshift(newItem);
  saveDb(db);
  return newItem;
}

export async function deleteInventoryItem(itemId) {
  await delay(250);

  const db = ensureDb();
  db.inventory = db.inventory.filter((item) => item.id !== itemId);
  saveDb(db);
}
