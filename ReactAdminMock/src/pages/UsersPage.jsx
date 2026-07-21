import { useEffect, useState } from 'react';
import { addUser, deleteUser, getUsers } from '../lib/mockApi.js';

const initialForm = {
  name: '',
  email: '',
  role: 'Manager',
  status: 'Active',
};

function UsersPage() {
  const [users, setUsers] = useState([]);
  const [form, setForm] = useState(initialForm);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadUsers = async () => {
    setLoading(true);
    const result = await getUsers();
    setUsers(result);
    setLoading(false);
  };

  useEffect(() => {
    loadUsers();
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
      await addUser(form);
      setForm(initialForm);
      await loadUsers();
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (userId) => {
    await deleteUser(userId);
    await loadUsers();
  };

  return (
    <div className="page-stack">
      <section className="grid-two">
        <article className="data-card">
          <div className="section-heading">
            <div>
              <p className="eyebrow">User management</p>
              <h3>Add user</h3>
            </div>
          </div>

          <form className="entity-form" onSubmit={handleSubmit}>
            <label>
              Name
              <input
                type="text"
                name="name"
                value={form.name}
                onChange={handleChange}
                placeholder="New team member"
                required
              />
            </label>

            <label>
              Email
              <input
                type="email"
                name="email"
                value={form.email}
                onChange={handleChange}
                placeholder="user@example.com"
                required
              />
            </label>

            <label>
              Role
              <select name="role" value={form.role} onChange={handleChange}>
                <option>Administrator</option>
                <option>Manager</option>
                <option>Analyst</option>
              </select>
            </label>

            <label>
              Status
              <select name="status" value={form.status} onChange={handleChange}>
                <option>Active</option>
                <option>Invited</option>
                <option>Paused</option>
              </select>
            </label>

            {error ? <p className="form-error">{error}</p> : null}

            <button type="submit" className="primary-button" disabled={isSubmitting}>
              {isSubmitting ? 'Saving user...' : 'Add user'}
            </button>
          </form>
        </article>

        <article className="data-card">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Directory</p>
              <h3>All users</h3>
            </div>
            <span className="metric-pill">{users.length} total</span>
          </div>

          {loading ? (
            <div className="loading-panel compact">Loading users...</div>
          ) : (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Role</th>
                    <th>Status</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map((user) => (
                    <tr key={user.id}>
                      <td>
                        <strong>{user.name}</strong>
                        <span>{user.email}</span>
                      </td>
                      <td>{user.role}</td>
                      <td>
                        <span className={`tag ${user.status.toLowerCase()}`}>{user.status}</span>
                      </td>
                      <td>
                        <button
                          type="button"
                          className="danger-button"
                          onClick={() => handleDelete(user.id)}
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

export default UsersPage;
