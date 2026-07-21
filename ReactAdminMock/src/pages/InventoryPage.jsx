import { useEffect, useState } from 'react';
import {
  addInventoryItem,
  deleteInventoryItem,
  getInventory,
} from '../lib/mockApi.js';

const initialForm = {
  itemName: '',
  sku: '',
  quantity: '',
  category: 'Electronics',
  price: '',
};

function InventoryPage() {
  const [items, setItems] = useState([]);
  const [form, setForm] = useState(initialForm);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadInventory = async () => {
    setLoading(true);
    const result = await getInventory();
    setItems(result);
    setLoading(false);
  };

  useEffect(() => {
    loadInventory();
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setIsSubmitting(true);

    try {
      await addInventoryItem(form);
      setForm(initialForm);
      await loadInventory();
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (itemId) => {
    await deleteInventoryItem(itemId);
    await loadInventory();
  };

  return (
    <div className="page-stack">
      <section className="grid-two">
        <article className="data-card">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Inventory flow</p>
              <h3>Add inventory</h3>
            </div>
          </div>

          <form className="entity-form" onSubmit={handleSubmit}>
            <label>
              Item name
              <input
                type="text"
                name="itemName"
                value={form.itemName}
                onChange={handleChange}
                placeholder="Product name"
                required
              />
            </label>

            <label>
              SKU
              <input
                type="text"
                name="sku"
                value={form.sku}
                onChange={handleChange}
                placeholder="SKU-1001"
                required
              />
            </label>

            <label>
              Quantity
              <input
                type="number"
                min="1"
                name="quantity"
                value={form.quantity}
                onChange={handleChange}
                placeholder="10"
                required
              />
            </label>

            <label>
              Category
              <select name="category" value={form.category} onChange={handleChange}>
                <option>Electronics</option>
                <option>Furniture</option>
                <option>Equipment</option>
                <option>Accessories</option>
              </select>
            </label>

            <label>
              Price
              <input
                type="number"
                min="1"
                step="0.01"
                name="price"
                value={form.price}
                onChange={handleChange}
                placeholder="149.99"
                required
              />
            </label>

            {error ? <p className="form-error">{error}</p> : null}

            <button type="submit" className="primary-button" disabled={isSubmitting}>
              {isSubmitting ? 'Saving item...' : 'Add inventory'}
            </button>
          </form>
        </article>

        <article className="data-card">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Stock table</p>
              <h3>Inventory list</h3>
            </div>
            <span className="metric-pill">{items.length} products</span>
          </div>

          {loading ? (
            <div className="loading-panel compact">Loading inventory...</div>
          ) : (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Item</th>
                    <th>SKU</th>
                    <th>Quantity</th>
                    <th>Price</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td>
                        <strong>{item.itemName}</strong>
                        <span>{item.category}</span>
                      </td>
                      <td>{item.sku}</td>
                      <td>{item.quantity}</td>
                      <td>${Number(item.price).toLocaleString()}</td>
                      <td>
                        <button
                          type="button"
                          className="danger-button"
                          onClick={() => handleDelete(item.id)}
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </article>
      </section>
    </div>
  );
}

export default InventoryPage;
