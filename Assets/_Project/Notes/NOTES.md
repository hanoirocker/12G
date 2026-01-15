NOVEDADES:
- Se movió la lógica de formateo a nuevo UIFormatter, que recibe las llamadas para actualizar los textos o parámetros de formato usando una nuevo estructura de datos de formato UIFormattingDataSO. Aplicado también a textos de interacción.

TOCHECK:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún se podría mencionar al inicio de Headaches el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar trueno al bajar por las escaleras en Unwired?
- Agregar mas sonidos stereo para los perfiles sonoros de ambiente?

TOFIX:
- Arreglar el error que se muestra en TV Time event sobre el TV Options no pudiendo actualizar su texto.
- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales --> Esto sucede cuando no existe extructura de audios de canales para el evento en particular.

TODO - próximo:
- REWORK REFERENCIAS: actualmente se buscan referencias en escena mediante "FindObjectOfType" .. tanto en el EventsHandler como en los eventos. \*BEINGADDRESED
  --> SOLUCION A TOMAR: Hacer singleton al EnvironmentHandler, PlayerHandler, PlayerHouseHandler, VirtualCamerasHandler y CinematicsHandler. Luego, reemplazar la búsqueda de estos por la referencia global inmedianta (ahorro de RAM sin necesidad de destruir corrutinas). \*DONE
  --> ACCIONES SECUNDARIAS: Se posibilita la eliminación de probablemente muchos GameEventSO's que solo se usaban para comunicarse entre un script y estos scripts. Tedioso pero innecesario a su vez (pueden seguir existiendo, aunque su presencia puede confundir a nivel código).
- Actualmente el sistema de audio por perfiles de ambiente funciona en base a la escena. Por ende, si quisieramos detener la lluvia en algun momento, la lluvia estereo del los perfiles seguiria sonando. Analizar si es conveniente o no ampliar la estructura logica.
- Finalizar Visions y comenzar con RedHour

TODO - despues:

- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar bolsas de basura.
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Agregar sonido para animación "Night - Stand Up".
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC

- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.
