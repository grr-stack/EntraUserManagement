import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth.js';

const navItems = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/users', label: 'Users' },
  { to: '/inventory', label: 'Inventory' },
];

function AppShell() {
  const { currentUser, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const pageTitle = navItems.find((item) => item.to === location.pathname)?.label ?? 'Workspace';

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand-block">
          <span className="brand-badge">RA</span>
          <div>
            <h1>Retail Axis</h1>
            <p>Mock admin platform</p>
          </div>
        </div>

        <nav className="nav-menu">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="sidebar-note">
          <h2>Testing Setup</h2>
          <p>
            This app uses a local mock API layer with persisted browser data so the flows
            behave like real async requests.
          </p>
        </div>
      </aside>

      <main className="main-panel">
        <header className="topbar">
          <div>
            <p className="eyebrow">Control center</p>
            <h2>{pageTitle}</h2>
          </div>

          <div className="topbar-actions">
            <div className="user-chip">
              <span>{currentUser?.name?.slice(0, 1) ?? 'A'}</span>
              <div>
                <strong>{currentUser?.name}</strong>
                <small>{currentUser?.role}</small>
              </div>
            </div>
            <button type="button" className="ghost-button" onClick={handleLogout}>
              Logout
            </button>
          </div>
        </header>

        <Outlet />
      </main>
    </div>
  );
}

export default AppShell;
