import { useState, useEffect } from 'react';
import { useLocation } from 'wouter';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { toast } from 'sonner';
import { Pencil, Trash2, Plus, Search } from 'lucide-react';
import { menuService, Menu } from '../services/menuService';

export default function SearchMenus() {
  const [, setLocation] = useLocation();
  const [searchTerm, setSearchTerm] = useState('');
  const [menus, setMenus] = useState<Menu[]>([]);
  const [loading, setLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  useEffect(() => {
    loadMenus();
  }, []);

  const loadMenus = async () => {
    try {
      setLoading(true);
      const data = await menuService.list();
      setMenus(data);
      setHasSearched(true);
    } catch (error) {
      toast.error('Erro ao carregar menus');
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      const data = await menuService.search(searchTerm);
      setMenus(data);
      setHasSearched(true);
      
      if (data.length === 0) {
        toast.info('Nenhum menu encontrado');
      }
    } catch (error) {
      toast.error('Erro ao buscar menus');
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number, nome: string) => {
    if (!confirm(`Deseja realmente excluir o menu "${nome}"?`)) {
      return;
    }

    try {
      await menuService.delete(id);
      toast.success('Menu excluído com sucesso');
      loadMenus();
    } catch (error) {
      toast.error('Erro ao excluir menu');
      console.error(error);
    }
  };

  const handleEdit = (id: number) => {
    setLocation(`/cadastro-menu?id=${id}`);
  };

  const handleNewMenu = () => {
    setLocation('/cadastro-menu');
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-6xl mx-auto">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-800">Cadastro Menu</h1>
          <Button
            onClick={handleNewMenu}
            className="bg-blue-600 hover:bg-blue-700 text-white flex items-center gap-2"
          >
            <Plus size={18} />
            Adicionar Novo
          </Button>
        </div>

        {/* Search Section */}
        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Nome do Menu:
              </label>
              <Input
                type="text"
                placeholder="Informe o Nome do Menu..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyPress={(e) => {
                  if (e.key === 'Enter') {
                    handleSearch();
                  }
                }}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div className="flex items-end">
              <Button
                onClick={handleSearch}
                disabled={loading}
                className="w-full bg-blue-600 hover:bg-blue-700 text-white flex items-center justify-center gap-2"
              >
                <Search size={18} />
                {loading ? 'Buscando...' : 'Busca'}
              </Button>
            </div>
          </div>
        </div>

        {/* Results Section */}
        {hasSearched && (
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            {loading ? (
              <div className="p-6 text-center text-gray-500">Carregando...</div>
            ) : menus.length === 0 ? (
              <div className="p-6 text-center text-gray-500">
                Nenhum menu encontrado
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-gray-100 border-b border-gray-200">
                    <tr>
                      <th className="px-6 py-3 text-left text-sm font-semibold text-gray-700">
                        Menu
                      </th>
                      <th className="px-6 py-3 text-left text-sm font-semibold text-gray-700">
                        Ordem
                      </th>
                      <th className="px-6 py-3 text-left text-sm font-semibold text-gray-700">
                        Ícone
                      </th>
                      <th className="px-6 py-3 text-left text-sm font-semibold text-gray-700">
                        Status
                      </th>
                      <th className="px-6 py-3 text-center text-sm font-semibold text-gray-700">
                        Opções
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {menus.map((menu) => (
                      <tr
                        key={menu.id}
                        className="border-b border-gray-200 hover:bg-gray-50 transition-colors"
                      >
                        <td className="px-6 py-4 text-sm text-gray-900">
                          {menu.nome}
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-900">
                          {menu.ordem}
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-900">
                          <i className={`fa ${menu.icone}`}></i> {menu.icone}
                        </td>
                        <td className="px-6 py-4 text-sm">
                          <span
                            className={`px-3 py-1 rounded-full text-xs font-medium ${
                              menu.status === 'Ativo'
                                ? 'bg-green-100 text-green-800'
                                : 'bg-red-100 text-red-800'
                            }`}
                          >
                            {menu.status}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-center">
                          <div className="flex justify-center gap-3">
                            <button
                              onClick={() => handleEdit(menu.id)}
                              className="text-blue-600 hover:text-blue-800 transition-colors"
                              title="Editar"
                            >
                              <Pencil size={18} />
                            </button>
                            <button
                              onClick={() => handleDelete(menu.id, menu.nome)}
                              className="text-red-600 hover:text-red-800 transition-colors"
                              title="Deletar"
                            >
                              <Trash2 size={18} />
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
