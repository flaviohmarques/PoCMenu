import { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { useLocation, useSearch } from 'wouter';
import { ArrowLeft } from 'lucide-react';
import { toast } from 'sonner';
import { menuService, CreateMenuDto, UpdateMenuDto } from '../services/menuService';

export default function CadastroMenu() {
  const [, setLocation] = useLocation();
  const search = useSearch();
  const params = new URLSearchParams(search);
  const menuId = params.get('id') ? parseInt(params.get('id')!) : null;

  const [formData, setFormData] = useState({
    nome: '',
    ordem: '',
    icone: '',
    descricao: '',
    status: 'Ativo' as 'Ativo' | 'Inativo',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);
  const [loadingMenu, setLoadingMenu] = useState(false);

  // Fetch existing menu if editing
  useEffect(() => {
    if (menuId) {
      loadMenu(menuId);
    }
  }, [menuId]);

  const loadMenu = async (id: number) => {
    try {
      setLoadingMenu(true);
      const menu = await menuService.getById(id);
      setFormData({
        nome: menu.nome,
        ordem: String(menu.ordem),
        icone: menu.icone,
        descricao: menu.descricao || '',
        status: menu.status,
      });
    } catch (error) {
      toast.error('Erro ao carregar menu');
      console.error(error);
    } finally {
      setLoadingMenu(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.nome.trim()) {
      newErrors.nome = 'O nome do menu é obrigatório';
    }

    const ordem = parseInt(formData.ordem);
    if (!formData.ordem || isNaN(ordem) || ordem <= 0) {
      newErrors.ordem = 'A ordem deve ser um número maior que zero';
    }

    if (!formData.icone.trim()) {
      newErrors.icone = 'O ícone do menu é obrigatório';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      toast.error('Por favor, corrija os erros no formulário');
      return;
    }

    try {
      setLoading(true);

      const menuData = {
        nome: formData.nome.trim(),
        ordem: parseInt(formData.ordem),
        icone: formData.icone.trim(),
        descricao: formData.descricao.trim() || undefined,
        status: formData.status,
      };

      if (menuId) {
        // Update
        await menuService.update(menuId, menuData as UpdateMenuDto);
        toast.success('Menu atualizado com sucesso!');
      } else {
        // Create
        await menuService.create(menuData as CreateMenuDto);
        toast.success('Menu incluído com sucesso!');
      }

      setLocation('/');
    } catch (error: any) {
      const errorMessage = error.message || 'Erro ao salvar menu';
      toast.error(errorMessage);
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: string, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    // Clear error for this field
    if (errors[field]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleBack = () => {
    setLocation('/');
  };

  if (loadingMenu) {
    return (
      <div className="min-h-screen bg-gray-50 p-6 flex items-center justify-center">
        <p className="text-gray-500">Carregando...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-3xl mx-auto">
        {/* Header */}
        <div className="flex items-center mb-6">
          <Button
            onClick={handleBack}
            variant="ghost"
            className="mr-4"
          >
            <ArrowLeft size={20} />
          </Button>
          <h1 className="text-3xl font-bold text-gray-800">
            {menuId ? 'Editar Menu' : 'Novo Menu'}
          </h1>
        </div>

        {/* Form */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Nome */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Nome do Menu: <span className="text-red-500">*</span>
              </label>
              <Input
                type="text"
                value={formData.nome}
                onChange={(e) => handleChange('nome', e.target.value)}
                placeholder="Digite o nome do menu"
                className={`w-full ${errors.nome ? 'border-red-500' : ''}`}
              />
              {errors.nome && (
                <p className="text-red-500 text-sm mt-1">{errors.nome}</p>
              )}
            </div>

            {/* Ordem */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Ordem: <span className="text-red-500">*</span>
              </label>
              <Input
                type="number"
                value={formData.ordem}
                onChange={(e) => handleChange('ordem', e.target.value)}
                placeholder="Digite a ordem"
                min="1"
                className={`w-full ${errors.ordem ? 'border-red-500' : ''}`}
              />
              {errors.ordem && (
                <p className="text-red-500 text-sm mt-1">{errors.ordem}</p>
              )}
            </div>

            {/* Ícone */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Ícone (Font Awesome): <span className="text-red-500">*</span>
              </label>
              <Input
                type="text"
                value={formData.icone}
                onChange={(e) => handleChange('icone', e.target.value)}
                placeholder="Ex: fa-home, fa-users"
                className={`w-full ${errors.icone ? 'border-red-500' : ''}`}
              />
              {errors.icone && (
                <p className="text-red-500 text-sm mt-1">{errors.icone}</p>
              )}
              <p className="text-gray-500 text-xs mt-1">
                Use classes do Font Awesome, ex: fa-home, fa-users, fa-cog
              </p>
            </div>

            {/* Descrição */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Descrição:
              </label>
              <textarea
                value={formData.descricao}
                onChange={(e) => handleChange('descricao', e.target.value)}
                placeholder="Digite uma descrição (opcional)"
                rows={3}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            {/* Status */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Status: <span className="text-red-500">*</span>
              </label>
              <select
                value={formData.status}
                onChange={(e) => handleChange('status', e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="Ativo">Ativo</option>
                <option value="Inativo">Inativo</option>
              </select>
            </div>

            {/* Buttons */}
            <div className="flex gap-4 pt-4">
              <Button
                type="submit"
                disabled={loading}
                className="flex-1 bg-blue-600 hover:bg-blue-700 text-white"
              >
                {loading ? 'Salvando...' : menuId ? 'Atualizar' : 'Incluir'}
              </Button>
              <Button
                type="button"
                onClick={handleBack}
                variant="outline"
                className="flex-1"
              >
                Cancelar
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
