# 🔵 Modelado del Dominio - TalentoPlus

## 📊 Diagrama de Entidades

```
┌─────────────────┐
│   Department    │
├─────────────────┤
│ + Id            │
│ + Name          │◄────────┐
│ + Code          │         │
│ + Description   │         │
└─────────────────┘         │
        │                   │
        │ 1                 │
        │                   │
        │ N                 │
        ▼                   │
┌─────────────────┐         │
│  JobPosition    │         │
├─────────────────┤         │
│ + Id            │         │
│ + Title         │         │
│ + Level         │         │
│ + MinSalary     │         │
│ + MaxSalary     │         │
│ + DepartmentId  │─────────┘
└─────────────────┘
        │
        │ 1
        │
        │ N
        ▼
┌─────────────────┐         ┌──────────────────┐
│    Employee     │────────►│ EducationLevel   │
├─────────────────┤   1:N   ├──────────────────┤
│ + Id            │         │ + Id             │
│ + DocumentNumber│         │ + LevelType      │
│ + FirstName     │         │ + DegreeName     │
│ + LastName      │         │ + Institution    │
│ + Email         │         │ + GraduationYear │
│ + PhoneNumber   │         │ + EmployeeId     │
│ + HireDate      │         └──────────────────┘
│ + Salary        │
│ + Status        │
│ + DepartmentId  │─────►Department
│ + JobPositionId │─────►JobPosition
└─────────────────┘
```

## 📋 Entidades Principales

### 1. **Employee** (Empleado)
Entidad central que representa un empleado de TalentoPlus.

**Información Personal:**
- `DocumentNumber`, `DocumentType`
- `FirstName`, `MiddleName`, `LastName`, `SecondLastName`
- `DateOfBirth`, `Gender`

**Información de Contacto:**
- `PersonalEmail`, `CorporateEmail`
- `PhoneNumber`, `AlternativePhoneNumber`
- `Address`, `City`, `Country`

**Información Laboral:**
- `HireDate`, `TerminationDate`
- `Salary`
- `Status` (Activo, Inactivo, Vacaciones, etc.)
- `ProfessionalProfile`

**Propiedades Calculadas:**
- `FullName`: Nombre completo concatenado
- `Age`: Edad calculada a partir de la fecha de nacimiento
- `YearsOfService`: Años de servicio en la empresa
- `IsCurrentlyActive`: Boolean que indica si está activo

**Métodos:**
- `GetHighestEducationLevel()`: Retorna el nivel educativo más alto

---

### 2. **Department** (Departamento)
Representa una división organizacional de la empresa.

**Propiedades:**
- `Name`: Nombre del departamento
- `Code`: Código único (ej: RRHH, TI, VEN)
- `Description`: Descripción del departamento

**Relaciones:**
- `Employees`: Colección de empleados
- `JobPositions`: Colección de cargos

---

### 3. **JobPosition** (Cargo)
Representa un cargo o posición laboral.

**Propiedades:**
- `Title`: Nombre del cargo
- `Level`: Nivel jerárquico
- `MinSalary`, `MaxSalary`: Rango salarial
- `Description`: Descripción del cargo

**Relaciones:**
- `Department`: Departamento al que pertenece
- `Employees`: Empleados que ocupan este cargo

---

### 4. **EducationLevel** (Nivel Educativo)
Representa la formación académica de un empleado.

**Propiedades:**
- `LevelType`: Enum (Primaria, Secundaria, Técnico, Pregrado, Maestría, Doctorado)
- `DegreeName`: Nombre del título
- `Institution`: Institución educativa
- `GraduationYear`: Año de graduación
- `FieldOfStudy`: Área de estudio

**Relaciones:**
- `Employee`: Empleado al que pertenece

---

### 5. **BaseEntity** (Clase Base)
Clase abstracta base para todas las entidades.

**Propiedades:**
- `Id`: Identificador único
- `CreatedAt`: Fecha de creación
- `UpdatedAt`: Fecha de última actualización
- `IsActive`: Soft delete flag

---

## 🔢 Enumeraciones

### **EmployeeStatus**
Estados posibles de un empleado:
- `Activo` = 1
- `Inactivo` = 2
- `Vacaciones` = 3
- `LicenciaMedica` = 4
- `Retirado` = 5

### **EducationLevelType**
Niveles educativos:
- `Ninguno` = 0
- `Primaria` = 1
- `Secundaria` = 2
- `Tecnico` = 3
- `Pregrado` = 4
- `Especializacion` = 5
- `Maestria` = 6
- `Doctorado` = 7

---

## 💎 Value Objects

### **Email**
Value object inmutable que representa un correo electrónico validado.

**Características:**
- Validación con expresión regular
- Normalización a minúsculas
- Factory method `Create()` con validación
- Factory method `TryCreate()` sin excepciones
- Sobrecarga de operadores de igualdad

**Ejemplo de uso:**
```csharp
var email = Email.Create("juan.perez@talentoplus.com");
var emailOrNull = Email.TryCreate(userInput);
```

---

### **PhoneNumber**
Value object inmutable que representa un número telefónico validado.

**Características:**
- Validación de formato
- Normalización de dígitos
- Longitud entre 7 y 15 dígitos
- Soporta formatos: +57 300 123 4567, 300-123-4567, etc.

**Ejemplo de uso:**
```csharp
var phone = PhoneNumber.Create("+57 300 123 4567");
var phoneOrNull = PhoneNumber.TryCreate(userInput);
```

---

## 🔌 Interfaces de Repositorio

### **IRepository<T>**
Repositorio genérico con operaciones CRUD:
- `GetAllAsync()`, `GetAllActiveAsync()`
- `GetByIdAsync(int id)`
- `AddAsync(T entity)`, `UpdateAsync(T entity)`
- `DeleteAsync(int id)`, `SoftDeleteAsync(int id)`
- `ExistsAsync(int id)`
- `SaveChangesAsync()`

---

### **IEmployeeRepository**
Repositorio específico para empleados con:

**Consultas básicas:**
- `GetByDocumentNumberAsync()`
- `GetByEmailAsync()`
- `GetByIdWithDetailsAsync()`

**Consultas por filtros:**
- `GetByDepartmentAsync()`
- `GetByJobPositionAsync()`
- `GetByStatusAsync()`
- `GetActiveEmployeesAsync()`

**Búsqueda y validación:**
- `SearchAsync(string searchTerm)`
- `DocumentNumberExistsAsync()`
- `EmailExistsAsync()`

**Estadísticas:**
- `GetEmployeeCountByDepartmentAsync()`
- `GetEmployeeCountByStatusAsync()`
- `GetHiredBetweenAsync()`

---

### **IDepartmentRepository**
Repositorio específico para departamentos:

**Consultas:**
- `GetByCodeAsync()`
- `GetByIdWithEmployeesAsync()`
- `GetByIdWithJobPositionsAsync()`
- `GetByIdWithDetailsAsync()`

**Búsqueda:**
- `SearchByNameAsync()`

**Validación:**
- `CodeExistsAsync()`
- `HasEmployeesAsync()`
- `HasJobPositionsAsync()`

**Estadísticas:**
- `GetDepartmentsWithEmployeeCountAsync()`

---

## 🎯 Reglas de Negocio del Dominio

### Empleado
1. ✅ El `DocumentNumber` debe ser único en el sistema
2. ✅ El `Email` debe ser único y estar validado
3. ✅ Un empleado debe pertenecer a un departamento y tener un cargo
4. ✅ El estado por defecto es `Activo`
5. ✅ El salario debe estar dentro del rango del cargo
6. ✅ `HireDate` no puede ser posterior a la fecha actual
7. ✅ Si `TerminationDate` está establecida, debe ser posterior a `HireDate`

### Departamento
1. ✅ El `Code` debe ser único
2. ✅ El `Name` es obligatorio
3. ✅ No se puede eliminar un departamento con empleados asignados

### Cargo (JobPosition)
1. ✅ `MinSalary` debe ser menor que `MaxSalary`
2. ✅ Debe estar asociado a un departamento
3. ✅ El `Level` indica la jerarquía (menor número = mayor jerarquía)

### Nivel Educativo
1. ✅ Un empleado puede tener múltiples niveles educativos
2. ✅ `GraduationYear` no puede ser futuro
3. ✅ El nivel más alto se determina por el valor del enum

---

## 📁 Estructura de Archivos

```
TalentoPlus.Domain/
├── Entities/
│   ├── BaseEntity.cs
│   ├── Employee.cs
│   ├── Department.cs
│   ├── JobPosition.cs
│   └── EducationLevel.cs
├── Enums/
│   ├── EmployeeStatus.cs
│   └── EducationLevelType.cs
├── ValueObjects/
│   ├── Email.cs
│   └── PhoneNumber.cs
└── Interfaces/
    ├── IRepository.cs
    ├── IEmployeeRepository.cs
    └── IDepartmentRepository.cs
```

---

## ✅ Estado del Modelado

| Componente | Estado | Archivos |
|------------|--------|----------|
| Entidades Base | ✅ Completado | 5 archivos |
| Enumeraciones | ✅ Completado | 2 archivos |
| Value Objects | ✅ Completado | 2 archivos |
| Interfaces | ✅ Completado | 3 archivos |
| Compilación | ✅ Sin errores | 0 warnings |

**Total de archivos**: 12 archivos de dominio + 1 proyecto (.csproj)

---

## 🚀 Próximos Pasos

- [ ] Configurar DbContext en Infrastructure
- [ ] Implementar repositorios concretos
- [ ] Crear configuraciones de Entity Framework
- [ ] Agregar validaciones con FluentValidation
- [ ] Crear migraciones de base de datos

---

**Última actualización**: Fase 2 - US-02 Completada ✅
