using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UpsaMe_API.Models;

namespace UpsaMe_API.Data.Seed
{
    public static class SubjectSeed
    {
        // Normaliza a slug ascii: sin tildes/ñ, minúsculas, guiones
        private static string ToSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string normalized = text.Trim().ToLowerInvariant();

            // Reemplazos comunes de acentos y caracteres especiales
            normalized = normalized
                .Replace('á', 'a').Replace('é', 'e').Replace('í', 'i')
                .Replace('ó', 'o').Replace('ú', 'u').Replace('ñ', 'n');

            // Cualquier cosa que no sea letra/dígito => espacio
            normalized = Regex.Replace(normalized, @"[^a-z0-9]+", "-");

            // Quita guiones duplicados y extremos
            normalized = Regex.Replace(normalized, @"-+", "-").Trim('-');

            return normalized;
        }

        // Aux: agrega materias a una carrera por nombre (idempotente por slug)
        private static void AddSubjectsForCareer(UpsaMeDbContext db, Career career, IEnumerable<string> subjectNames)
        {
            var existingSlugs = db.Subjects
                                  .Where(s => s.CareerId == career.Id)
                                  .Select(s => s.Slug!)
                                  .ToHashSet();

            var toAdd = new List<Subject>();
            foreach (var name in subjectNames.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                var slug = ToSlug(name);
                if (existingSlugs.Contains(slug)) continue;

                toAdd.Add(new Subject
                {
                    Id = Guid.NewGuid(),
                    Name = name.Trim(),
                    Slug = slug,
                    Code = null,            // si luego quieres, generamos códigos
                    CareerId = career.Id
                });
            }

            if (toAdd.Count > 0)
            {
                db.Subjects.AddRange(toAdd);
            }
        }

        // Mapa carrera -> listado de materias (TODAS las que enviaste)
        private static Dictionary<string, string[]> MapCareerSubjects() => new()
        {
            // ============= ADU =============
            ["arquitectura"] = new[]
            {
                // 1
                "Diseño I","Representación Espacial","Expresión Gráfica","Matemáticas","Diseño, Construcción y Materiales","Pensamiento Crítico",
                // 2
                "Diseño II","Morfología I","Geografía, Sitio y Contexto","Estática de las Estructuras","Construcción de Viviendas","Interculturalidad, Ciudadanía y Género",
                // 3
                "Diseño III","Morfología II","Teoría ADU","Instalaciones Hidrosanitarias","Estructuras de Acero y Madera",
                // 4
                "Diseño IV","Diseño & Multimedia I","Historia de la Arquitectura y Urbanismo I","Instalaciones Eléctricas","Construcción de Edificios","Estructuras de Hormigón Armado",
                // 5
                "Diseño V","Morfología III","Historia de la Arquitectura y Urbanismo II","Instalaciones Complementarias",
                // 6
                "Diseño VI","Diseño Urbano I","Arquitectura Contemporánea","Administración de Proyectos ADU",
                // 7
                "Diseño VII","Diseño Urbano II","Arquitectura y Urbanismo Boliviano","Economía, Planificación y Gestión ADU",
                // 8
                "Diseño VIII","Planificación Urbana","Ética y Legislación ADU","Diseño Tecnológico"
            },
            ["diseno-industrial"] = new[]
            {
                // 1
                "Diseño Industrial I","Representación Espacial","Expresión Gráfica","Matemáticas","Diseño, Materiales e Industria","Pensamiento Crítico",
                // 2
                "Diseño Industrial II","Morfología I","Historia del Arte","Innovación y Creatividad","Diseño y Ergonomía","Diseño y Mecánica",
                // 3
                "Diseño Industrial III","Morfología II","Teoría ADU","Diseño, Electricidad y Electrónica","Modelado y Maquetería",
                // 4
                "Diseño Industrial IV","Diseño & Multimedia I","Arte y Diseño Contemporáneo","Semiótica y Marketing ADU","Diseño, Mecatrónica y Domótica",
                // 5
                "Diseño Industrial V","Morfología III","Diseño y Producción Industrial","Interculturalidad, Ciudadanía y Género",
                // 6
                "Diseño Industrial VI","Administración de Proyectos ADU","Diseño & Multimedia II",
                // 7
                "Diseño Industrial VII","Economía, Planificación y Gestión ADU","Diseño Industrial Contemporáneo",
                // 8
                "Modalidad de Graduación I","Ética y Legislación ADU","Liderazgo y Competitividad"
            },

            // ============= EMPRESARIALES =============
            ["adm-empresas"] = new[]
            {
                // 1
                "Contabilidad General I","Introducción a la Economía","Fundamentos de Matemática","Álgebra I","Pensamiento Crítico","Fundamentos de Administración de Empresas",
                // 2
                "Contabilidad General II","Historia y Geografía Económica","Álgebra II","Métodos y Técnicas de Investigación","Cálculo I","Teoría de la Organización",
                // 3
                "Contabilidad de Costos I","Ofimática Avanzada","Estadística I","Microeconomía I","Fundamentos de Marketing","Organización y Sistemas",
                // 4
                "Matemática Financiera I","Estadística II","Macroeconomía I","Microeconomía II","Derecho Comercial, Laboral y Tributario",
                // 5
                "Administración Financiera I","Ética y Gestión con Valores","Investigación Operativa I","Administración Pública","Comunicación Corporativa",
                // 6
                "Presupuesto","Interculturalidad, Ciudadanía y Género","Emprendimiento y Gestión de la Innovación","Administración de Sistemas de Información","Administración de Recursos Humanos",
                // 7
                "Software Especializado","Administración de la Producción","Resolución de Problemas y Toma de Decisiones","Gestión de Empresas Familiares y PYMES","Administración Financiera II",
                // 8
                "Mercado de Valores e Instrumentos Financieros","Elaboración y Evaluación de Proyectos","Gestión de Calidad","Estrategia Empresarial","Dirección Estratégica","Negociación y Gestión de Conflictos",
                // 9
                "Proyectos de Planificación Estratégica","Desarrollo Organizacional",
                // 10
                "Gestión por Competencias"
            },
            ["auditoria-finanzas"] = new[]
            {
                // 1
                "Contabilidad General I","Fundamentos y Administración de Empresas","Introducción a la Economía","Fundamentos de Matemáticas","Álgebra I","Pensamiento Crítico",
                // 2
                "Contabilidad General II","Teoría de la Organización","Métodos y Técnicas de Investigación","Cálculo I","Álgebra II","Historia y Geografía Económica",
                // 3
                "Contabilidad de Costos I","Organización y Sistemas","Ofimática Avanzada","Microeconomía I","Estadística I",
                // 4
                "Matemática Financiera I","Contabilidad de Costos II","Macroeconomía I","Microeconomía II","Derecho Comercial, Laboral y Tributario","Estadística II",
                // 5
                "Administración Financiera I","Contabilidad y Costos Agropecuarios","Ética y Gestión con Valores","Contabilidad y Costos Petroleros","Interculturalidad, Ciudadanía y Género","Contabilidad de Entidades Financieras",
                // 6
                "Emprendimiento y Gestión de la Innovación","Presupuesto","Normas Contables","Gabinete Contable y Tributario","Administración de Recursos Humanos","Administración de Sistemas de Información",
                // 7
                "Principios y Normas de Auditoría","Informática Contable","Taller de Tributos a la Renta y al Consumo","Administración Financiera II","Resolución de Problemas y Toma de Decisiones",
                // 8
                "Auditoría Operativa","Mercado de Valores e Instrumentos Financieros","Estrategia Empresarial","Elaboración y Evaluación de Proyectos",
                // 9
                "Gabinete de Auditoría","Contabilidad Gubernamental",
                // 10
                "Auditoría Forense","Auditoría Fiscal y Tributaria"
            },
            ["comercio-int"] = new[]
            {
                // 1
                "Introducción a la Economía","Contabilidad General I","Fundamentos de Administración de Empresas","Fundamentos de Matemática","Álgebra I","Pensamiento Crítico",
                // 2
                "Historia y Geografía Económica","Contabilidad General II","Teoría de la Organización","Cálculo I","Álgebra II","Métodos y Técnicas de Investigación",
                // 3
                "Microeconomía I","Contabilidad de Costos I","Interculturalidad, Ciudadanía y Género","Ofimática Avanzada","Estadística I",
                // 4
                "Microeconomía II","Macroeconomía I","Integración Económica","Matemáticas Financieras I","Derecho Comercial, Laboral y Tributario","Estadística II",
                // 5
                "Economía Internacional","Teoría del Comercio Internacional","Administración Financiera I","Conciliación y Arbitraje Internacional","Ética y Gestión con Valores","Investigación de Mercados I",
                // 6
                "Marketing Internacional","Distribución Física Internacional","Administración de Recursos Humanos","Presupuesto","Derecho Internacional Público","Legislación, Verificación y Valoración Aduanera",
                // 7
                "Incoterms y Clasificación Arancelaria","Resolución de Problemas y Toma de Decisiones","Administración Financiera II","Derecho Internacional Privado",
                // 8
                "Taller de Comercio Internacional","Internacionalización de la Empresa","Estrategia Empresarial","Mercado de Valores e Instrumentos Financieros","Elaboración y Evaluación de Proyectos",
                // 9
                "Proyectos de Exportación e Importación","Técnicas de Negociación Internacional",
                // 10
                "Finanzas Internacionales"
            },
            ["ing-comercial"] = new[]
            {
                // 1
                "Contabilidad General I","Introducción a la Economía","Fundamentos de Administración de Empresas","Pensamiento Crítico","Álgebra I","Fundamentos de Matemática",
                // 2
                "Contabilidad General II","Historia y Geografía Económica","Métodos y Técnicas de Investigación","Interculturalidad, Ciudadanía y Género","Álgebra II","Cálculo I",
                // 3
                "Contabilidad de Costos I","Microeconomía I","Fundamentos de Marketing","Estadística I","Ofimática Avanzada",
                // 4
                "Matemática Financiera I","Microeconomía II","Macroeconomía I","Derecho Comercial, Laboral y Tributario","Análisis del Comportamiento del Consumidor","Estadística II",
                // 5
                "Administración Financiera I","Investigación Operativa I","Negociación y Ventas","Publicidad y Comunicación","Investigación de Mercado I","Ética y Gestión con Valores",
                // 6
                "Presupuesto","Marketing de Servicios","Innovación y Desarrollo de Productos","Trade Marketing","Marketing Internacional",
                // 7
                "Administración Financiera II","Política de Precios","Econometría I","Software Especializado","Resolución de Problemas y Toma de Decisiones",
                // 8
                "Distribución y Logística","Estrategia Empresarial","Marketing Estratégico","Investigación de Mercados II","Elaboración y Evaluación de Proyectos",
                // 9
                "Modelación Comercial","Emprendimiento y Creación de Empresas",
                // 10
                "Cadena de Suministro"
            },
            ["ing-economica"] = new[]
            {
                // 1
                "Introducción a la Economía","Contabilidad General I","Fundamentos de Administración de Empresas","Fundamentos de Matemática","Álgebra I","Pensamiento Crítico",
                // 2
                "Historia y Geografía Económica","Contabilidad General II","Teoría de la Organización","Cálculo I","Álgebra II","Métodos y Técnicas de Investigación",
                // 3
                "Microeconomía I","Contabilidad de Costos I","Cálculo II","Ofimática Avanzada","Estadística I",
                // 4
                "Microeconomía II","Macroeconomía I","Matemática Financiera I","Derecho Comercial, Laboral y Tributario","Interculturalidad, Ciudadanía y Género","Estadística II",
                // 5
                "Microeconomía III","Macroeconomía II","Teoría del Comercio Internacional","Administración Financiera I","Ética y Gestión con Valores","Investigación Operativa I",
                // 6
                "Macroeconomía de Economías Abiertas","Economía de la Empresa","Teoría de los Juegos","Presupuesto","Emprendimiento y Gestión de la Innovación","Investigación Operativa II",
                // 7
                "Economía Fiscal","Organización Industrial","Econometría I","Resolución de Problemas y Toma de Decisiones","Administración Financiera II",
                // 8
                "Economía Monetaria","Economía Agropecuaria","Econometría II","Estrategia Empresarial","Elaboración y Evaluación de Proyectos",
                // 9
                "Política Económica","Economía del Medio Ambiente","Economía Financiera",
                // 10
                "Historia del Pensamiento Económico"
            },
            ["ing-financiera"] = new[]
            {
                // 1
                "Pensamiento Crítico","Fundamentos de Matemática","Álgebra I","Introducción a la Economía","Fundamentos de Administración de Empresas","Contabilidad General I",
                // 2
                "Teoría de la Organización","Métodos y Técnicas de Investigación","Cálculo I","Álgebra II","Historia y Geografía Económica","Contabilidad General II",
                // 3
                "Ofimática Avanzada","Estadística I","Cálculo II","Microeconomía I","Contabilidad de Costos I",
                // 4
                "Estadística II","Macroeconomía I","Microeconomía II","Derecho Comercial, Laboral y Tributario","Matemática Financiera I",
                // 5
                "Investigación Operativa I","Contabilidad de Entidades Financieras","Administración Financiera I","Matemática Financiera II","Interculturalidad, Ciudadanía y Género","Ética y Gestión con Valores",
                // 6
                "Administración de Recursos Humanos","Emprendimiento y Gestión de la Innovación","Análisis de Información Financiera","Presupuesto","Economía Bancaria y de Seguro",
                // 7
                "Econometría I","Administración Financiera II","Resolución de Problemas y Toma de Decisiones","Regulación Financiera","Finanzas Públicas","Software Especializado",
                // 8
                "Elaboración y Evaluación de Proyectos","Estrategia Empresarial","Administración y Finanzas Agropecuaria","Finanzas de PYMES","Mercado de Valores e Instrumentos Financieros",
                // 9
                "Fusión y Adquisición de Negocios","Administración de Riesgo","Economía Financiera",
                // 10
                "Finanzas Internacionales","Modelación Financiera"
            },
            ["marketing"] = new[]
            {
                // 1
                "Contabilidad General I","Introducción a la Economía","Fundamentos de Administración de Empresas","Edición Digital de Imágenes","Lenguaje Escrito y Visual","Pensamiento Crítico",
                // 2
                "Contabilidad General II","Historia y Geografía Económica","Teoría de la Organización","Creatividad","Métodos y Técnicas de Investigación","Matemática",
                // 3
                "Contabilidad de Costos I","Microeconomía I","Fundamentos de Marketing","Fotografía","Estadística I",
                // 4
                "Matemática Financiera I","Microeconomía II","Macroeconomía I","Derecho Comercial, Laboral y Tributario","Fundamentos de Publicidad y Propaganda","Estadística II",
                // 5
                "Administración Financiera I","Investigación de Mercado I","Dirección de Arte y Redacción Publicitaria","Innovación y Publicidad","Interculturalidad, Ciudadanía y Género",
                // 6
                "Presupuesto","Marketing Internacional","Marketing de Servicios","Trade Marketing","Producción Audiovisual",
                // 7
                "Canales de Distribución y Ventas","Diseño Web y Multimedia","Protocolo y Eventos","Creación y Gestión de Marcas","Ética y Gestión con Valores",
                // 8
                "Administración de Ventas","Marketing Estratégico","Investigación de Mercados II","Medios Sociales y Digitales",
                // 9
                "Estrategia de Publicidad","Modalidad de Graduación I",
                // 10
                "Creación y Dirección de Agencia"
            },

            // ============= JURÍDICAS =============
            ["derecho"] = new[]
            {
                // 1
                "Derecho Informático","Lenguaje y Oratoria","Historia del Derecho","Teoría General del Derecho","Derecho Civil I","Criminología",
                // 2
                "Economía Política","Pensamiento Crítico","Derecho Constitucional I","Derecho Civil II","Derecho Penal I",
                // 3
                "Análisis Económico del Derecho","Ética Jurídica","Ciencia Política","Derecho Constitucional II","Derecho Civil III","Derecho Penal II",
                // 4
                "Derecho Agrario y Forestal","Sociología Jurídica","Interculturalidad, Ciudadanía y Género","Ley de Organización Judicial","Derecho Civil IV","Derecho Procesal Penal",
                // 5
                "Derecho Ambiental","Filosofía Jurídica","Derecho Laboral y de la Seguridad Social","Derecho Administrativo","Derecho Procesal Civil","Medicina Legal y Psiquiatría Forense",
                // 6
                "Derecho Comercial I","Lógica y Argumentación Jurídica","Derecho Internacional Privado","Derecho Municipal","Derecho de Familia y de la Niñez","Práctica Forense Penal",
                // 7
                "Derecho Comercial II","Inglés Legal","Derecho Internacional Público","Derecho Procesal Constitucional","Práctica Forense Civil",
                // 8
                "Derecho Minero e Hidrocarburos","Investigación Jurídica","Derecho Financiero y Tributario","Defensa Legal del Estado","Derecho Civil V",
                // 9 (Menciones)
                "Derecho Económico","Comercio Exterior","Fundamentos de Administración de Empresas","Fundamentos de Contabilidad Comercial","Derecho de Autor, Registro, Marcas y Patentes",
                "Relaciones Internacionales","Derecho Comunitario y de la Integración","Conciliación y Arbitraje Internacional","Derecho Internacional de los Derechos Humanos","Derecho de los Negocios Internacionales"
            },

            // ============= HUMANIDADES =============
            ["comunicacion"] = new[]
            {
                // 1
                "Fundamentos de Administración de Empresas","Edición Digital de Imágenes","Fundamentos de la Comunicación","Redacción y Estilo","Pensamiento Crítico","Investigación Documental",
                // 2
                "Teoría de la Organización","Diagramación Digital","Teorías de Comunicación Corporativa","Redacción Periodística","Seminario Filosófico","Estadística Básica",
                // 3
                "Organización y Sistemas","Fundamentos de Marketing","Fotografía","Redacción Corporativa","Interculturalidad, Ciudadanía y Género","Semiología",
                // 4
                "Fundamentos de Publicidad y Propaganda","Auditorías de Imagen y Comunicación","Gestión de Prensa Corporativa","Realidad Nacional","Metodología de Investigación Cuantitativa",
                // 5
                "Gestión de la Marca","Planificación Estratégica","Audiovisual Corporativo","Economía y Sociedad Internacional","Metodología de Investigación Cualitativa",
                // 6
                "Administración de Recursos Humanos","Protocolo y Eventos","Gestión de Contenidos Web","Paradigmas y Discursos Globales","Comunicación y Cambio Social",
                // 7
                "Contabilidad y Presupuesto","Campaña Publicitaria","Sectores, Comunicación y Crisis","Gestión de Comunidades Virtuales","Ética y Responsabilidad Social","Comunicación y Política",
                // 8
                "Proyecto de Empresas","Comunicación para Gobiernos","Apreciación Estética","Investigación en Comunicación",
                // 9
                "Asesorías y Consultorías en Comunicación"
            },
            ["psicologia"] = new[]
            {
                // 1
                "Fundamentos de Administración de Empresas","Historia de la Psicología","Psicología General","Ofimática Avanzada","Investigación Documental","Pensamiento Crítico",
                // 2
                "Teoría de la Organización","Procesos Cognitivos Básicos","Personalidad","Psicobiología","Estadística Básica",
                // 3
                "Organización y Sistemas","Aprendizaje","Psicología Social I","Neuropsicología","Interculturalidad, Ciudadanía y Género","Psicoestadística",
                // 4
                "Derecho Social","Psicología Educativa","Psicología Social II","Psicología del Desarrollo I","Metodología de la Investigación Cuantitativa",
                // 5
                "Comunicación Corporativa","Destrezas Básicas de la Entrevista","Sexología","Psicología del Desarrollo II","Metodología de la Investigación Cualitativa","Teoría de los Tests",
                // 6
                "Administración de Recursos Humanos","Psicodiagnóstico","Psicopatología I","Psicología Comunitaria","Dinámica Grupal","Psicología Experimental",
                // 7
                "Evaluación de Recursos Humanos","Tests Psicométricos","Psicopatología II","Teoría y Clínica Psicoanalítica","Psicología Jurídica",
                // 8
                "Negociación y Gestión de Conflictos","Terapia Cognitivo Conductual","Terapias Humanistas","Técnicas Proyectivas","Orientación e Intervención Sistémica",
                // 9
                "Desarrollo Organizacional",
                // 10
                "Gestión por Competencias"
            },
            ["diseno-grafico"] = new[]
            {
                // 1
                "Fundamentos de Comunicación Visual","Edición Digital de Imágenes","Dibujo para Diseño Gráfico","Morfología y Composición","Pensamiento Crítico","Fundamentos de Administración de Empresas",
                // 2
                "Lenguaje de la Imagen","Tipografía","Diagramación Digital","Ilustración Digital","Historia del Arte","Creatividad",
                // 3
                "Imagen y Comunicación Corporativa","Fundamentos del Color","Fotografía","Redacción Creativa","Semiología","Fundamentos de Marketing",
                // 4
                "Fundamentos de Publicidad y Propaganda","Materiales, Pre-prensa e Impresión","Animación","Arte y Diseño Contemporáneo","Interculturalidad, Ciudadanía y Género",
                // 5
                "Gestión de la Marca","Diseño de Marcas","Diseño Editorial","Audiovisual Corporativo","Investigación Documental","Arte y Diseño Latinoamericano","Certificación de Inglés",
                // 6
                "Dirección de Arte y Creatividad Publicitaria","Identidad Visual Corporativa","Señalética","Diseño para Web y Multimedia","Apreciación Estética",
                // 7
                "Campaña Publicitaria","Diseño de Empaques","Diseño para Redes Sociales y Aplicaciones Móviles","Metodología Proyectual","Ética y Responsabilidad Social","Contabilidad y Presupuesto",
                // 8
                "Diseño de Carteles y Vía Pública","Tendencias e Innovación en Diseño","Proyecto de Empresas",
                // 9
                "Asesorías y Consultorías en Comunicación"
            },
            ["diseno-moda"] = new[]
            {
                // 1
                "Moldería y Sastrería I","Dibujo de Figurín I","Diseño de Portafolio","Informática y Moda I","Historia de la Moda","Pensamiento Crítico",
                // 2
                "Moldería y Sastrería II","Dibujo de Figurín II","Tecnología de Materiales I","Informática y Moda II","Creatividad","Arte y Diseño Contemporáneo",
                // 3
                "Moldería y Sastrería III","Diseño de Indumentaria Empresarial","Tecnología de Materiales II","Fundamentos del Color","Semiología","Fundamentos de Administración de Empresas",
                // 4
                "Confección Industrial","Diseño de Indumentaria Casual","Diseño de Accesorios","Interculturalidad, Ciudadanía y Género","Sociología del Consumo","Fundamentos de Marketing",
                // 5
                "Procesos Productivos","Diseño de Colección Prêt-à-Porter","Fotografía","Imagen y Comunicación Corporativa","Investigación Documental",
                // 6
                "Diseño de Colección Niño","Diseño de Lencería y Ropa de Baño","Producción de Moda","Trajes y Tradiciones Bolivianas","Investigación de Tendencias","Contabilidad, Costos y Presupuesto",
                // 7
                "Diseño de Trajes de Gala","Exhibición de Productos de Moda","Ética y Responsabilidad Social","Metodología Proyectual","Proyecto de Empresas",
                // 8
                "Diseño de Vestuario para Espectáculo","Asesoría de Imagen Personal","Comunicación para la Moda"
            },

            // ============= INGENIERÍA =============
            ["ing-civil"] = new[]
            {
                // 1
                "Representación Gráfica I","Fundamentos de Matemática","Álgebra I","Pensamiento Crítico","Geología para Ingeniería Civil",
                // 2
                "Representación Gráfica II","Álgebra II","Cálculo I","Mecánica","Fundamentos de Programación","Interculturalidad, Ciudadanía y Género",
                // 3
                "Topografía","Estructuras Isostáticas","Cálculo II","Estadística I","Fluidos, Calor y Sonido","Química General",
                // 4
                "Sistema de Información Geográfica","Mecánica de las Estructuras","Introducción a la Simulación de Modelos","Cálculo III","Electromagnetismo y Ondas","Materiales de Construcción",
                // 5
                "Análisis Estructural I","Métodos Numéricos","Tecnología del Hormigón","Instalaciones Eléctricas","Hidráulica",
                // 6
                "Carreteras I","Análisis Estructural II","Mecánica de Suelos","Ingeniería Civil y Medio Ambiente","Construcción de Edificios","Hidrología",
                // 7
                "Carreteras II","Hormigón Armado I","Fundaciones","Ordenamiento del Territorio","Ética y Seguridad en la Construcción","Instalaciones en Edificios",
                // 8
                "Ingeniería de Tráfico y Transporte","Hormigón Armado II","Análisis Estructural III","Dirección de Obras","Obras de Saneamiento Básico",
                // 9
                "Ferrocarriles","Puentes y Hormigón Pretensado","Estructuras de Acero","Gerencia y Contratación de Obras","Obras Hidráulicas"
            },
            ["ing-sistemas"] = new[]
            {
                // 1
                "Fundamentos de Matemática","Álgebra I","Contabilidad General I","Pensamiento Crítico","Fundamentos de TI","Fundamentos de Administración de Empresas",
                // 2
                "Cálculo I","Álgebra II","Contabilidad General II","Fundamentos de Programación","Redes I","Electrónica Digital",
                // 3
                "Cálculo II","Estadística I","Contabilidad de Costos I","Estructura de Datos","Redes II","Sistemas Digitales",
                // 4
                "Cálculo III","Estadística II","Matemática Financiera I","Programación Aplicada","Sistemas Operativos","Interculturalidad, Ciudadanía y Género",
                // 5
                "Investigación Operativa I","Análisis Estadístico de Calidad","Bases de Datos","Estructura de Datos Avanzadas","Laboratorio de Sistemas Operativos I","Laboratorio Redes I",
                // 6
                "Investigación Operativa II","Análisis para la Toma de Decisiones","Ingeniería de Software","Laboratorio de Sistemas Operativos II","Ética y Valores",
                // 7
                "Modelación y Simulación","Desarrollo de Aplicaciones Web","Sistemas de Información","Computación Móvil y Ubicua","Virtualización y Computación en la Nube","Laboratorio Redes II",
                // 8
                "Evaluación de Proyectos de Ingeniería","Gestión de la Calidad Total","Gestión de Bases de Datos","Ingeniería de Calidad y Pruebas",
                // 9
                "Sistemas Inteligentes e Innovación","Gestión de Sistemas"
            },
            ["ing-mecatronica"] = new[]
            {
                // 1
                "Fundamentos de Matemática","Dibujo Técnico","Álgebra I","Fundamentos de TI","Pensamiento Crítico","Fundamentos de Administración de Empresas",
                // 2
                "Cálculo I","Mecánica","Álgebra II","Redes I","Electrónica Digital","Fundamentos de Programación",
                // 3
                "Cálculo II","Fluidos, Calor y Sonido","Química General","Redes II","Sistemas Digitales","Estructura de Datos",
                // 4
                "Cálculo III","Circuitos I","Resistencia de Materiales","Electrónica I","Programación Aplicada","Interculturalidad, Ciudadanía y Género",
                // 5
                "Ingeniería de Control I","Termodinámica","Mecánica de Fluidos","Electrónica II","Bases de Datos",
                // 6
                "Ingeniería de Control II","Circuitos II","Estadística I","Elementos Mecánicos","Instrumentación I","Ética y Valores",
                // 7
                "Robótica Básica","Máquinas Eléctricas I","Tecnología de Equipos y Máquinas","Instrumentación II",
                // 8
                "Robótica Industrial","Máquinas Eléctricas II","Diseño de Productos y Servicios","Sistemas en Tiempo Real",
                // 9
                "Robótica Móvil","Diseño Mecatrónico","Manufactura Integrada por Computadora"
            },
            ["ing-informatica"] = new[]
            {
                // 1
                "Fundamentos de Matemática","Álgebra I","Fundamentos de Administración de Empresas","Pensamiento Crítico","Fundamentos de TI","Contabilidad General I",
                // 2
                "Cálculo I","Álgebra II","Teoría de la Organización","Fundamentos de Programación","Redes I","Contabilidad General II",
                // 3
                "Cálculo II","Estadística I","Interculturalidad, Ciudadanía y Género","Estructuras de Datos","Redes II","Contabilidad de Costos I",
                // 4
                "Ética y Valores","Estadística II","Programación Aplicada","Sistemas Operativos","Matemática Financiera I",
                // 5
                "Investigación Operativa I","Análisis Estadístico de Calidad","Bases de Datos","Estructura de Datos Avanzadas","Laboratorio de Sistemas Operativos I","Administración Financiera I",
                // 6
                "Emprendimiento y Gestión de la Innovación","Administración de Recursos Humanos","Ingeniería de Software","Laboratorio de Sistemas Operativos II","Presupuesto","Análisis para la Toma de Decisiones",
                // 7
                "Sistemas de Información","Desarrollo de Aplicaciones Web","Virtualización y Computación en la Nube","Resolución de Problemas y Toma de Decisiones","Gestión de Empresas Familiares y PYMES",
                // 8
                "Gestión de Calidad Total","Gestión de Base de Datos","Estrategia Empresarial","Evaluación de Proyectos de Ingeniería","Dirección Estratégica",
                // 9
                "Sistemas Inteligentes e Innovación","Gestión de Sistemas","Proyectos de Planificación Estratégica"
            },
            ["ing-industrial"] = new[]
            {
                // 1
                "Fundamentos de Matemática","Álgebra I","Dibujo Técnico","Fundamentos de Administración de Empresas","Contabilidad General I","Pensamiento Crítico",
                // 2
                "Cálculo I","Álgebra II","Mecánica","Teoría de la Organización","Contabilidad General II","Fundamentos de Programación",
                // 3
                "Cálculo II","Química General","Fluidos, Calor y Sonido","Estadística I","Contabilidad de Costos I","Estructura de Datos",
                // 4
                "Cálculo III","Resistencia de Materiales","Electromagnetismo y Ondas","Estadística II","Matemática Financiera I","Programación Aplicada",
                // 5
                "Termodinámica","Mecánica de Fluidos","Análisis Estadístico de Calidad","Investigación Operativa I","Interculturalidad, Ciudadanía y Género","Base de Datos",
                // 6
                "Fisicoquímica","Instalaciones Eléctricas Industriales","Elementos Mecánicos","Administración de la Producción y Operaciones I","Investigación Operativa II","Análisis para la Toma de Decisiones",
                // 7
                "Transferencia de Masa y Calor","Seguridad y Mantenimiento Industrial","Tecnología de Equipos y Máquinas","Administración de la Producción y Operaciones II","Modelación y Simulación","Sistemas de Información",
                // 8
                "Diseño de Sistemas de Producción","Diseño de Productos y Servicios","Gestión Ambiental","Evaluación de Proyectos de Ingeniería","Gestión de Calidad Total",
                // 9
                "Gestión de la Innovación Tecnológica y Emprendimiento","Modalidad de Graduación I","Ética y Valores"
            },
        };

        public static void Seed(UpsaMeDbContext db)
        {
            // Trae carreras una vez
            var careers = db.Careers.AsNoTracking().ToList();
            if (!careers.Any())
            {
                Console.WriteLine("⚠️ No hay carreras aún. Ejecuta primero el seed de Faculties/Careers.");
                return;
            }

            var map = MapCareerSubjects();
            int totalAdded = 0;

            foreach (var kv in map)
            {
                var careerSlug = kv.Key;
                var subjects = kv.Value;

                var career = careers.FirstOrDefault(c => c.Slug == careerSlug);
                if (career == null)
                {
                    Console.WriteLine($"⚠️ Carrera con slug '{careerSlug}' no encontrada. Verifica CareerSeed.");
                    continue;
                }

                int before = db.Subjects.Count(s => s.CareerId == career.Id);
                AddSubjectsForCareer(db, career, subjects);
                int after = before + db.ChangeTracker.Entries<Subject>().Count(e => e.State == EntityState.Added && e.Entity.CareerId == career.Id);

                totalAdded += (after - before);
                Console.WriteLine($"→ {career.Name}: agregadas {(after - before)} materias nuevas.");
            }

            if (totalAdded > 0)
            {
                db.SaveChanges();
                Console.WriteLine($"✅ Materias cargadas/actualizadas. Nuevas: {totalAdded}");
            }
            else
            {
                Console.WriteLine("ℹ️ No había materias nuevas para agregar.");
            }
        }
    }
}
